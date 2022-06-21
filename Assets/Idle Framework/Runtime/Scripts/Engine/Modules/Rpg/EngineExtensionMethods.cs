using BreakInfinity;
using io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Combat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg
{
    public static class EngineExtensionMethods
    {
        private static Dictionary<ValueContainer, Character> WrappedCharacterCache = new Dictionary<ValueContainer, Character>();
        public static AttackResultDescription MakeAttack(this IdleEngine engine, Character attacker, Character defender)
        {
            BigDouble attackDamage = engine.CalculateAttackDamage(attacker, defender);
            defender.CurrentHealth = BigDouble.Max(0, defender.CurrentHealth - attackDamage);
            var context = new Dictionary<string, object>()
            {
                { "attacker", attacker },
                { "defender", defender }
            };
            int attackRoll = engine.RandomInt(100) + 1;
            BigDouble attackRollTarget = BigDouble.Max(0, BigDouble.Min(100, attacker.Accuracy - defender.Evasion + 50)); // FIXME: Set base to-hit change
            if (attackRoll <= attackRollTarget)
            {
                ((ValueContainer)attacker).NotifyImmediately(AttackHitEvent.EventName, context, attacker, attackDamage);
                ((ValueContainer)attacker).NotifyImmediately(DamageInflictedEvent.EventName, context, attacker, attackDamage);

                ((ValueContainer)defender).NotifyImmediately(HitByAttackEvent.EventName, context, attacker, attackDamage);
                ((ValueContainer)defender).NotifyImmediately(DamageTakenEvent.EventName, context, defender, attacker, attackDamage);
                return new AttackResultDescription(true);
            } else
            {
                ((ValueContainer)attacker).NotifyImmediately(AttackMissEvent.EventName, context, attacker, attackDamage);

                ((ValueContainer)defender).NotifyImmediately(MissedByAttackEvent.EventName, context, attacker, attackDamage);
                return new AttackResultDescription(false);
            }
        }

        public static BigDouble CalculateAttackDamage(this IdleEngine engine, Character attacker, Character defender)
        {
            BigDouble attackerDamage = attacker.Damage;

            return attackerDamage;
        }

        public static Character AsCharacter(this ValueContainer targetContainer)
        {
            Character character;
            if(!WrappedCharacterCache.TryGetValue(targetContainer, out character))
            {
                character = new Character(targetContainer);
                targetContainer.Engine.RegisterMethod(string.Format("{0}CharacterAct", targetContainer.Id), character.Act);
                character.Subscribe("character", CharacterActedEvent.EventName, string.Format("{0}CharacterAct", targetContainer.Id), true);
                WrappedCharacterCache[targetContainer] = character;
            }
            return character;
        }

        public static void ApplyStatus(this IdleEngine engine,Character character, string statusId, float duration)
        {
            var statusDefinition = engine.GetDefinition<StatusDefinition>("status", statusId);
            if(statusDefinition == null)
            {
                throw new InvalidOperationException();
            }
            var characterStatuses = ((ValueContainer)character).GetProperty(Character.Attributes.STATUSES).ValueAsMap();
            if(!characterStatuses.ContainsKey(statusId))
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
            if(characterStatuses.ContainsKey(statusId))
            {
                ((ValueContainer)character).RemoveModifier(statusDefinition);
            }
            characterStatuses[statusId].Set((string)null);
        }

        private static EncounterDefinition GetRandomEncounter(this IdleEngine engine)
        {
            var encounters = engine.GetDefinitions<EncounterDefinition>("encounter");
            if (encounters != null) {
                var options = encounters.ToList();
                int index = engine.RandomInt(options.Count);
                return options[index];
            }
            return null;
        }

        /*
         * Start a new encounter.
         * 
         * If an encounter definition is given, use that definition to generate the encounter. Otherwise, use a random one.
         */
        public static void StartEncounter(this IdleEngine engine, EncounterDefinition encounter = null) {
            engine.GetProperty("encounter", IdleEngine.GetOperationType.GET_OR_CREATE).Set(new Dictionary<string, ValueContainer>()
            {
                { "creatures", engine.CreateValueContainer(new List<ValueContainer>()) }
            });
            if(encounter == null)
            {
                encounter = engine.GetRandomEncounter();
            }
            foreach(var option in encounter.CreatureOptions)
            {
                var creatureDefinition = engine.GetDefinition<CreatureDefinition>("creature", option.Item1);
                if(creatureDefinition == null)
                {
                    throw new InvalidOperationException();
                }
                var level = engine.GetProperty("level").ValueAsNumber() + option.Item2;
                var creature = engine.GenerateCreature(creatureDefinition, level);
                engine.GetProperty("encounter.creatures").ValueAsList().Add(creature);
                creature.GetProperty("id").Set(creature.Id);
            }
        }

        private static Character GenerateCharacter(this IdleEngine engine)
        {
            var character = engine.CreateValueContainer(new Dictionary<string, ValueContainer>()).AsCharacter();
            return character;
        }

        public static Character GenerateCreature(this IdleEngine engine, CreatureDefinition creatureDefinition, BigDouble level)
        {
            var character = engine.GenerateCharacter();
            character.CurrentHealth = character.MaximumHealth = (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.MAXIMUM_HEALTH] as string);
            character.Damage = (BigDouble)engine.EvaluateExpression("return " + creatureDefinition.Properties[Character.Attributes.DAMAGE] as string);
            character.GetProperty("level").Set(level);
            return character;
        }

        public static Character GeneratePlayer(this IdleEngine engine)
        {
            var character = engine.GenerateCharacter();
            character.CurrentHealth = 25; // FIXME: Configure base health
            character.Damage = 1;
            return character;
        }

        public static bool InActiveEncounter(this IdleEngine engine)
        {
            return EncounterIsActive(engine.GetProperty("encounter").ValueAsMap(), engine.GetProperty("player").ValueAsMap());
        }

        private static bool EncounterIsActive(IDictionary<string, ValueContainer> encounter, IDictionary<string, ValueContainer> player)
        {
            if (encounter == null)
            {
                return false;
            }
            var monsters = encounter["monsters"].ValueAsMap();
            bool anyMonsterAlive = false;
            foreach (var monster in monsters)
            {
                ;
                anyMonsterAlive = anyMonsterAlive || !monster.Value.AsCharacter().IsAlive;
            }
            bool playerAlive = player[Character.Attributes.CURRENT_HEALTH].ValueAsNumber() > 0;
            return anyMonsterAlive && playerAlive;
        }
    }
}