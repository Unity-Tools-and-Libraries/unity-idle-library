using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public static class EngineExtensionMethods
    {
        private static Dictionary<ValueContainer, Character> WrappedCharacterCache = new Dictionary<ValueContainer, Character>();
        public static AttackResultDescription MakeAttack(this IdleEngine engine, Character attacker, Character defender)
        {
            BigDouble attackDamage = engine.CalculateAttackDamage(attacker, defender);
            var context = new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender }
            };
            int attackRoll = engine.RandomInt(100) + 1;
            
            BigDouble attackRollTarget = BigDouble.Max(0, BigDouble.Min(100, attacker.Accuracy - defender.Evasion + 50)); // FIXME: Set base to-hit change
            if (attackRoll <= attackRollTarget)
            {
                int criticalRoll = engine.RandomInt(1000) + 1;
                BigDouble criticalRollTarget = BigDouble.Max(0, BigDouble.Min(1000, attacker.Precision - defender.Resilience));

                if (criticalRoll <= criticalRollTarget)
                {
                    engine.Log(LogType.Log, () => String.Format("Character {0} critically hit character {1} with an attack: rolled {2}/{4} vs {3}/{5}", attacker.Id, defender.Id, attackRoll, attackRollTarget, criticalRoll, criticalRollTarget), "character.combat");
                    attackDamage = attackDamage * attacker.CriticalHitDamageMultiplier;
                    ((ValueContainer)attacker).NotifyImmediately(AttackHitEvent.EventName, context, attacker, attackDamage);
                    ((ValueContainer)defender).NotifyImmediately(HitByAttackEvent.EventName, context, attacker, attackDamage);

                    defender.InflictDamage(attackDamage, attacker);

                    return new AttackResultDescription(true, "critical hit");
                } else
                {
                    engine.Log(LogType.Log, () => String.Format("Character {0} hit character {1} with an attack: rolled {2} vs {3}", attacker.Id, defender.Id, attackRoll, attackRollTarget), "character.combat");
                    ((ValueContainer)attacker).NotifyImmediately(AttackHitEvent.EventName, context, attacker, attackDamage);
                    ((ValueContainer)defender).NotifyImmediately(HitByAttackEvent.EventName, context, attacker, attackDamage);

                    defender.InflictDamage(attackDamage, attacker);

                    return new AttackResultDescription(true, "critical hit");
                }
            }
            else
            {
                engine.Log(LogType.Log, () => String.Format("Character {0} missed character {1} with an attack: rolled {2} vs {3}", attacker.Id, defender.Id, attackRoll, attackRollTarget), "character.combat");
                ((ValueContainer)attacker).NotifyImmediately(AttackMissEvent.EventName, context, attacker, "Miss!");

                ((ValueContainer)defender).NotifyImmediately(MissedByAttackEvent.EventName, context, attacker, attackDamage);
                return new AttackResultDescription(false, "miss");
            }
        }

        public static BigDouble CalculateXpValue(this IdleEngine engine, Character character)
        {
            return (BigDouble)engine.EvaluateExpression(engine.GetProperty("configuration.xp_calculation_method").ValueAsString(), new Dictionary<string, object>()
            {
                { "character", character }
            });
        }

        public static BigDouble CalculateGoldValue(this IdleEngine engine, Character character)
        {
            return (BigDouble)engine.EvaluateExpression(engine.GetProperty("configuration.gold_calculation_method").ValueAsString(), new Dictionary<string, object>()
            {
                { "character", character }
            });
        }

        public static IDictionary<string, ValueContainer> GetCurrentEncounter(this IdleEngine engine)
        {
            return engine.GetProperty("encounter").ValueAsMap();
        }

        public static string SetActionPhase(this IdleEngine engine, string actionPhase)
        {
            return engine.GetProperty(RpgModule.Properties.ActionPhase).Set(actionPhase);
        }

        public static BigDouble CalculateAttackDamage(this IdleEngine engine, Character attacker, Character defender)
        {
            BigDouble attackerDamage = attacker.Damage;

            return attackerDamage;
        }

        public static Character AsCharacter(this ValueContainer targetContainer)
        {
            Character character;
            if (!WrappedCharacterCache.TryGetValue(targetContainer, out character))
            {
                character = new Character(targetContainer);
                targetContainer.Engine.RegisterMethod(string.Format("{0}CharacterAct", targetContainer.Id), character.Act);
                WrappedCharacterCache[targetContainer] = character;
            }
            return character;
        }

        public static void ApplyStatus(this IdleEngine engine, Character character, string statusId, float duration)
        {
            var statusDefinition = engine.GetDefinition<StatusDefinition>("status", statusId);
            if (statusDefinition == null)
            {
                throw new InvalidOperationException();
            }
            var characterStatuses = ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES).ValueAsMap();
            if (!characterStatuses.ContainsKey(statusId))
            {
                ((ValueContainer)character).AddModifier(statusDefinition);
            }
            ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES + "." + statusId, IdleEngine.GetOperationType.GET_OR_CREATE).Set(duration);
        }

        public static void RemoveStatus(this IdleEngine engine, Character character, string statusId)
        {
            var statusDefinition = engine.GetDefinition<StatusDefinition>("status", statusId);
            if (statusDefinition == null)
            {
                throw new InvalidOperationException();
            }
            var characterStatuses = ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES).ValueAsMap();
            if (characterStatuses.ContainsKey(statusId))
            {
                ((ValueContainer)character).RemoveModifier(statusDefinition);
            }
            characterStatuses[statusId].Set((string)null);
        }

        private static EncounterDefinition GetRandomEncounter(this IdleEngine engine)
        {
            var encounters = engine.GetDefinitions<EncounterDefinition>("encounter");
            if (encounters != null)
            {
                var options = encounters.ToList();
                int index = engine.RandomInt(options.Count);
                return options[index];
            }
            throw new InvalidOperationException();
        }

        /*
         * Start a new encounter.
         * 
         * If an encounter definition is given, use that definition to generate the encounter. Otherwise, use a random one.
         */
        public static void StartEncounter(this IdleEngine engine, EncounterDefinition encounter = null)
        {
            var encounterCreatures = engine.CreateValueContainer(new List<ValueContainer>());
            engine.GetProperty("encounter", IdleEngine.GetOperationType.GET_OR_CREATE).Set(new Dictionary<string, ValueContainer>()
            {
                { "creatures", encounterCreatures }
            });
            if (encounter == null)
            {
                encounter = engine.GetRandomEncounter();
            }
            foreach (var option in encounter.CreatureOptions)
            {
                var creatureDefinition = engine.GetDefinition<CreatureDefinition>("creature", option.Item1);
                if (creatureDefinition == null)
                {
                    throw new InvalidOperationException();
                }
                var level = engine.GetProperty("level").ValueAsNumber() + option.Item2;
                var creature = engine.GenerateCreature(creatureDefinition, level);
                creature.GetProperty("id").Set(creature.Id);
                creature.GetProperty(Character.Attributes.ACTION).Set(Character.Actions.FIGHT);
                encounterCreatures.ValueAsList().Add(creature);
            }
            engine.NotifyImmediately(EncounterStartedEvent.EventName, null, null, encounterCreatures);
        }
        
        public static Character GetRandomTarget(this IdleEngine engine, BigDouble friendlyParty)
        {
            if(friendlyParty == 0)
            {
                var monsters = engine.GetProperty("encounter.creatures").ValueAsList();
                int index = engine.RandomInt(monsters.Count);
                return monsters[index].AsCharacter();
            } else
            {
                return engine.GetProperty("player").AsCharacter();
            }
        }

        public static Character GetPlayer(this IdleEngine engine)
        {
            return engine.GetProperty("player").AsCharacter();
        }

        private static Character GenerateCharacter(this IdleEngine engine)
        {
            var character = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()).AsCharacter();
            character.CriticalHitChance = 0;
            character.Accuracy = 0;
            character.Evasion = 0;
            character.Defense = 0;
            character.Penetration = 0;
            character.Resilience = 0;
            return character;
        }

        public static Character GenerateCreature(this IdleEngine engine, CreatureDefinition creatureDefinition, BigDouble level)
        {
            var character = engine.GenerateCharacter();
            character.CurrentHealth = character.MaximumHealth = (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.MAXIMUM_HEALTH] as string);
            character.Damage = (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.DAMAGE] as string);
            character.Icon = creatureDefinition.Properties[Character.Attributes.ICON] as string;
            character.Party = 1;
            return character;
        }

        public static Character GeneratePlayer(this IdleEngine engine)
        {
            var character = engine.GenerateCharacter();
            character.Party = 0;
            character.MaximumHealth = character.CurrentHealth = 25; // FIXME: Configure base health
            character.Damage = 1;
            character.Xp = 0;
            character.Gold = 0;

            character.GetProperty("known_powers", IdleEngine.GetOperationType.GET_OR_CREATE).Set(new Dictionary<string, ValueContainer>());

            engine.CreateProperty("player", (ValueContainer)character);
            return character;
        }

        public static bool InActiveEncounter(this IdleEngine engine)
        {
            var encounter = engine.GetProperty("encounter");
            return encounter != null && EncounterIsActive(encounter.ValueAsMap(), engine.GetProperty("player").ValueAsMap());
        }

        private static bool EncounterIsActive(IDictionary<string, ValueContainer> encounter, IDictionary<string, ValueContainer> player)
        {
            if (encounter == null)
            {
                return false;
            }
            var monsters = encounter["creatures"].ValueAsList();
            bool anyMonsterAlive = false;
            foreach (var monster in monsters)
            {
                anyMonsterAlive = anyMonsterAlive || !monster.AsCharacter().IsAlive;
            }
            bool playerAlive = player[Character.Attributes.CURRENT_HEALTH].ValueAsNumber() > 0;
            return anyMonsterAlive && playerAlive;
        }
    }
}