using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace DangerousCombat
{
    public class Main : MBSubModuleBase
    {
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
            InformationManager.DisplayMessage(new InformationMessage("Mission Started at: "));
            InformationManager.DisplayMessage(new InformationMessage(mission.SceneName));
            mission.AddMissionBehaviour(new DangerousMissionBehaviour());
            
        }
    }
    
    public class DangerousMissionBehaviour: MissionBehaviour
    {
        private Dictionary<Agent, float> HeroHealth;
        private Random rand;
        public DangerousMissionBehaviour()
        {
            InformationManager.DisplayMessage(new InformationMessage("Initializing Dangerous Mission Behavior"));
            BehaviourType = MissionBehaviourType.Other;
            HeroHealth = new Dictionary<Agent, float>();
            rand = new Random();
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {
            base.OnAgentBuild(agent, banner);
            if (agent.IsHero)
            {
                InformationManager.DisplayMessage(new InformationMessage(agent.Name + " being added to dictionary"));
                HeroHealth.Add(agent, agent.Health);
            }
            
        }

        public override void AfterStart()
        {
            base.AfterStart();
            foreach (Agent currentAgent in Mission.Current.Agents)
            {
                if (currentAgent.IsHero)
                {
                    // if (((CharacterObject) currentAgent.Character).HeroObject != null)
                    // {
                    //     Hero hero = ((CharacterObject) currentAgent.Character).HeroObject;
                    //     InformationManager.DisplayMessage(new InformationMessage(hero.Name.ToString() + " found as hero"));
                    // }
                    // Hero hero = ((CharacterObject) currentAgent.Character).HeroObject;
                    // Hero hero = currentAgent.
                    
                    

                }
                
            }
        }

        public override void OnRegisterBlow(Agent attacker, Agent victim, GameEntity realHitEntity, Blow b,
            ref AttackCollisionData collisionData)
        {
            base.OnRegisterBlow(attacker, victim, realHitEntity, b, ref collisionData);
            if (victim.IsHero)
            {
                // Hero hero = ((CharacterObject) victim.Character).HeroObject;
                // InformationManager.DisplayMessage(new InformationMessage(hero.Name.ToString() + " found as hero"));

                InformationManager.DisplayMessage(
                    new InformationMessage(
                        "On Register blow: " +
                        attacker.Name +
                        " hit " +
                        victim.Name +
                        " with " +
                        HeroHealth[victim].ToString()  + " health remaining after " +
                        b.InflictedDamage.ToString() + " damage was inflicted and " +
                        b.AbsorbedByArmor.ToString() + " damage absorbed by armor" // b.VictimBodyPart. + " "

                    ));
                if (victim.Health == 0)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            attacker.Name + " finished " +
                            victim.Name + " with " +
                            HeroHealth[victim].ToString() + " health remaining after " +
                            b.InflictedDamage.ToString() + " damage was inflicted and " +
                            b.AbsorbedByArmor.ToString() + " damage absorbed by armor"

                        ));
                    var excess_damage = b.InflictedDamage - HeroHealth[victim];
                    excess_damage = excess_damage * excess_damage;
                    var random_roll = rand.NextDouble();
                    var wound_threshold = .25;
                    var death_threshold = .1;
                    var body_part_multiplier = 1.0;
                    var damage_scaler = .0015;
                    var damage_type_multiplier = 1.0;
                    switch (b.DamageType)
                    {
                        case DamageTypes.Blunt:
                            damage_type_multiplier = .5;
                            break;
                        case DamageTypes.Cut:
                            damage_type_multiplier = .75;
                            break;
                        case DamageTypes.Pierce:
                            damage_type_multiplier= 1.0 ;
                            break;
                        default:
                            damage_type_multiplier = .4;
                            break;
                    }
                    switch (b.VictimBodyPart)
                    {
                        case BoneBodyPartType.Head:
                            body_part_multiplier = 1.0;
                            break;
                        case BoneBodyPartType.Neck:
                            body_part_multiplier = .9;
                            break;
                        case BoneBodyPartType.Chest:
                            body_part_multiplier = .5;
                            break;
                        case BoneBodyPartType.Abdomen:
                            body_part_multiplier = .3;
                            break;
                        case BoneBodyPartType.ShoulderLeft:
                            body_part_multiplier = .2;
                            break;
                        case BoneBodyPartType.ShoulderRight:
                            body_part_multiplier = .2;
                            break;
                        case BoneBodyPartType.BipedalLegs:
                            body_part_multiplier = .1;
                            break;
                        case BoneBodyPartType.BipedalArmLeft:
                            body_part_multiplier = .1;
                            break;
                        case BoneBodyPartType.BipedalArmRight:
                            body_part_multiplier = .1;
                            break;
                        default:
                            body_part_multiplier = 0.0;
                            break;
                    }
                    InformationManager.DisplayMessage(new InformationMessage(random_roll.ToString() + " was random roll"));

                    if (excess_damage * damage_scaler * body_part_multiplier * damage_type_multiplier * death_threshold 
                    > random_roll)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(victim.Name + " has been killed"));
                        InformationManager.DisplayMessage(new InformationMessage("Excess Damage " + excess_damage.ToString() +
                                                                                 " Damage Scaler " + damage_scaler +
                                                                                 " body part " + body_part_multiplier +
                                                                                 " damage type " + damage_type_multiplier));
                        InformationManager.DisplayMessage(new InformationMessage("Chance of death was " + (excess_damage * damage_scaler * body_part_multiplier * damage_type_multiplier * death_threshold).ToString() ));
                        // Hero hero = ((CharacterObject) victim.Character).HeroObject;
                        // InformationManager.DisplayMessage(new InformationMessage(hero.Name.ToString() + " found as hero"));

                        // KillCharacterAction.ApplyByBattle((Hero)victim.Character., true);
                        // MBObjectManager.Instance.GetObject<Hero>(victim.Character.StringId).Name.ToString();

                    }
                    else if (excess_damage * damage_scaler * body_part_multiplier * damage_type_multiplier * wound_threshold 
                        > random_roll)
                    {
                        InformationManager.DisplayMessage(new InformationMessage(victim.Name + " has been wounded"));
                        InformationManager.DisplayMessage(new InformationMessage("Excess Damage " + excess_damage.ToString() +
                                                                                 " Damage Scaler " + damage_scaler +
                                                                                 " body part " + body_part_multiplier +
                                                                                 " damage type " + damage_type_multiplier));
                        InformationManager.DisplayMessage(new InformationMessage("Chance of wound was " + (excess_damage * damage_scaler * body_part_multiplier * damage_type_multiplier * wound_threshold).ToString() ));


                    }
                }
                HeroHealth[victim] = victim.Health;
            }
            
        }

        public override MissionBehaviourType BehaviourType { get; }
    }
}
