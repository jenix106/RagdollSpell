using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace RagdollSpell
{
    public class RagdollOptions : ThunderScript
    {
        [ModOptionDontSave]
        [ModOptionButton]
        [ModOption(name: "Repossess Player", tooltip: "Repossesses the player character after being ragdolled", valueSourceName: nameof(repossessOption), defaultValueIndex = 1)]
        public static void RepossessPlayer(bool value)
        {
            if(value && Player.local?.creature == null && Player.characterData != null)
            {
                Player.local.StartCoroutine(SpawnPlayerCreature());
            }
        }
        [ModOption(name: "Kill Ragdoll", tooltip: "Kills the creature once it ragdolls", valueSourceName: nameof(booleanOption), defaultValueIndex = 0)]
        public static bool KillRagdoll;
        public static IEnumerator SpawnPlayerCreature()
        {
            Vector3 position = Player.local.transform.position;
            Quaternion rotation = Player.local.transform.rotation;
            yield return PlayerSpawner.current?.SpawnCoroutine();
            Player.local.Teleport(position, rotation);
        }
        public static ModOptionBool[] repossessOption =
        {
            new ModOptionBool("Repossess", true),
            new ModOptionBool("Repossess", false)
        };
        public static ModOptionBool[] booleanOption =
        {
            new ModOptionBool("Enabled", true),
            new ModOptionBool("Disabled", false)
        };
    }
    public class RagdollSpell : SpellCastCharge
    {
        public override void Fire(bool active)
        {
            base.Fire(active);
            if (active)
            {
                Fire(false);
                if (Player.local.creature != null)
                {
                    Creature creature = Player.local.creature;
                    creature.handRight.TryRelease();
                    creature.handLeft.TryRelease();
                    creature.handLeft.caster.telekinesis.TryRelease();
                    creature.handRight.caster.telekinesis.TryRelease();
                    Player.local.ReleaseCreature();
                    creature.ragdoll.SetState(Ragdoll.State.Inert);
                    creature.lastInteractionTime = Time.time;
                    creature.fallState = Creature.FallState.NearGround;
                    creature.brain.Stop();
                    creature.autoEyeClipsActive = false;
                    creature.locomotion.ClearPhysicModifiers();
                    creature.locomotion.ClearSpeedModifiers();
                    creature.spawnGroup = null;
                    if (RagdollOptions.KillRagdoll) creature.Kill();
                }
            }
        }
    }
}
