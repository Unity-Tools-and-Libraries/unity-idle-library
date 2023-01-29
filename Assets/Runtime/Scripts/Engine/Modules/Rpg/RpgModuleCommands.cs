using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace io.github.thisisnozaku.idle.framework.Engine.Modules.Rpg.Commands
{
    public class RpgModuleCommands
    {
        public static void KillCharacter(IdleEngine engine, string[] args)
        {
            long id = CommandArgumentParser.Parse<long>("id", args[1]);
            if (id == 1L)
            {
                engine.GetPlayer<RpgPlayer>().Character.Kill();
            }
            else
            {
                var encounter = engine.GetCurrentEncounter();
                foreach (var character in encounter.Creatures)
                {
                    if (character.Id == id)
                    {
                        character.Kill();
                        return;
                    }
                }
                throw new Exception(String.Format("No character with id {0} found", id));
            }
        }

        public static void DamageCharacter(IdleEngine engine, string[] args)
        {
            long id = CommandArgumentParser.Parse<long>("id", args[1]);
            long damage = CommandArgumentParser.Parse<long>("id", args[2]);
            if (id == 1L)
            {
                engine.GetPlayer<RpgPlayer>().Character.InflictDamage(damage, null);
            }
            else
            {
                var encounter = engine.GetCurrentEncounter();
                foreach (var character in encounter.Creatures)
                {
                    if (character.Id == id)
                    {
                        character.InflictDamage(damage, null);
                        return;
                    }
                }
                throw new Exception(String.Format("No character with id {0} found", id));
            }
        }
    }
}