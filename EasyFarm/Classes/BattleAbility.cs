﻿/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using EasyFarm.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Parsing.Abilities;
using Parsing.Types;

namespace EasyFarm.Classes
{
    /// <summary>
    ///     An ability that should be used in battle. It represents both
    ///     targeted and buffing abilities allowing the player to choose
    ///     to cast abilities when a buff has worn.
    /// </summary>
    public class BattleAbility : BindableBase
    {
        /// <summary>
        ///     Maps ability type objects to their commands.
        /// </summary>
        private static readonly Dictionary<AbilityType, string> CommandMapper
            = new Dictionary<AbilityType, string>();

        /// <summary>
        ///     Resource information about the move.
        /// </summary>
        private Ability _ability = new Ability();

        private AbilityType _abilityType = AbilityType.Unknown;

        /// <summary>
        ///     The move's max distance.
        /// </summary>
        private double _distance = Constants.MeleeDistance;

        /// <summary>
        ///     Is this move ready for use?
        /// </summary>
        private bool _isEnabled;

        /// <summary>
        ///     The move's name.
        /// </summary>
        private string _name = string.Empty;

        /// <summary>
        ///     The upper limit of the player's health.
        /// </summary>
        private int _playerLowerHealth;

        /// <summary>
        ///     The lower limit of the player's health.
        /// </summary>
        private int _playerUpperHealth;

        /// <summary>
        ///     The status effect to check for.
        /// </summary>
        private string _statusEffect = string.Empty;

        /// <summary>
        ///     Target's lower health threshold.
        /// </summary>
        private int _targetLowerHealth;

        /// <summary>
        ///     Regular expression used for filtering target names.
        /// </summary>
        private string _targetName = string.Empty;

        /// <summary>
        ///     Target's upper health threshold.
        /// </summary>
        private int _targetUpperHealth;

        /// <summary>
        ///     Check for the presense or absents of a status effect.
        ///     Unchecked means we use moves in absence of the status effect.
        ///     Checked means we use moves when the status effect is present.
        /// </summary>
        private bool _triggerOnEffectPresent;

        /// <summary>
        ///     Private backing for UsageLimit.
        /// </summary>
        private int _usageLimit;

        /// <summary>
        ///     Private backing for usages.
        /// </summary>
        private int _usages;

        static BattleAbility()
        {
            var commandTypes = new ObservableCollection<AbilityType>
            {
                AbilityType.Unknown,
                AbilityType.Jobability,
                AbilityType.Magic,
                AbilityType.Monsterskill,
                AbilityType.Ninjutsu,
                AbilityType.Pet,
                AbilityType.Range,
                AbilityType.Song,
                AbilityType.Weaponskill
            };

            // Load valid prefixes.
            CommandPrefixes = new ReadOnlyObservableCollection<AbilityType>(commandTypes);

            var commandPrefix = new ObservableCollection<TargetType>
            {
                TargetType.Unknown,
                TargetType.Self,
                TargetType.Enemy
            };

            // Load valid targets. 
            CommandTargets = new ReadOnlyObservableCollection<TargetType>(commandPrefix);

            // Ability type objects to their commands. 
            CommandMapper.Add(AbilityType.Jobability, "/jobability");
            CommandMapper.Add(AbilityType.Magic, "/magic");
            CommandMapper.Add(AbilityType.Monsterskill, "/monsterskill");
            CommandMapper.Add(AbilityType.Ninjutsu, "/ninjutsu");
            CommandMapper.Add(AbilityType.Pet, "/pet");
            CommandMapper.Add(AbilityType.Range, "/range");
            CommandMapper.Add(AbilityType.Song, "/song");
            CommandMapper.Add(AbilityType.Weaponskill, "/weaponskill");
        }

        /// <summary>
        ///     Create our command binds and initialize our user's
        ///     move usage conditions.
        /// </summary>
        public BattleAbility() : this(new Ability())
        {
            AutoFillCommand = new DelegateCommand(AutoFill);
        }

        /// <summary>
        ///     Create a storing a reference to the given ability.
        /// </summary>
        /// <param name="ability"></param>
        public BattleAbility(Ability ability)
        {
            Ability = ability;
            Name = ability.English;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                Ability.English = _name;
            }
        }

        public double Distance
        {
            get { return _distance; }
            set
            {
                SetProperty(ref _distance, (int) value);
                AppInformer.InformUser("Distance set to {0}.", _distance);
            }
        }

        /// <summary>
        ///     Holds the resource file information for the move.
        /// </summary>
        public Ability Ability
        {
            get { return _ability; }
            set { SetProperty(ref _ability, value); }
        }

        public bool TriggerOnEffectPresent
        {
            get { return _triggerOnEffectPresent; }
            set { SetProperty(ref _triggerOnEffectPresent, value); }
        }

        public string StatusEffect
        {
            get { return _statusEffect; }
            set { SetProperty(ref _statusEffect, value); }
        }

        public int PlayerLowerHealth
        {
            get { return _playerLowerHealth; }
            set
            {
                SetProperty(ref _playerLowerHealth, value);
                AppInformer.InformUser("Lower health set to {0}.", _playerLowerHealth);
            }
        }

        public int PlayerUpperHealth
        {
            get { return _playerUpperHealth; }
            set
            {
                SetProperty(ref _playerUpperHealth, value);
                AppInformer.InformUser("Upper health set to {0}.", _playerUpperHealth);
            }
        }

        public string TargetName
        {
            get { return _targetName; }
            set { SetProperty(ref _targetName, value); }
        }

        public int TargetLowerHealth
        {
            get { return _targetLowerHealth; }
            set
            {
                SetProperty(ref _targetLowerHealth, value);
                AppInformer.InformUser("Lower health set to {0}.", _targetLowerHealth);
            }
        }

        public int TargetUpperHealth
        {
            get { return _targetUpperHealth; }
            set
            {
                SetProperty(ref _targetUpperHealth, value);
                AppInformer.InformUser("Upper health set to {0}.", _targetUpperHealth);
            }
        }

        public AbilityType AbilityType
        {
            get { return Ability.AbilityType; }
            set
            {
                SetProperty(ref _abilityType, value);

                var prefix = CommandMapper.ContainsKey(value)
                    ? CommandMapper[value]
                    : string.Empty;

                Ability.Prefix = prefix;
            }
        }

        /// <summary>
        ///     The maximum limit of times this move can be used in battle.
        /// </summary>
        public int UsageLimit
        {
            get { return _usageLimit; }
            set { SetProperty(ref _usageLimit, value); }
        }

        /// <summary>
        ///     The number of times this move has been used.
        /// </summary>
        public int Usages
        {
            get { return _usages; }
            set { SetProperty(ref _usages, value); }
        }

        /// <summary>
        ///     Sets an BattleAbility's ability object using the ability service object
        ///     for lookups.
        /// </summary>
        // Delegate Commands cannot be serialized. 
        [XmlIgnore]
        public DelegateCommand AutoFillCommand { get; set; }

        /// <summary>
        ///     List of usable command prefixes.
        /// </summary>
        public static ReadOnlyObservableCollection<AbilityType> CommandPrefixes { get; set; }

        /// <summary>
        ///     List of usable command targets.
        /// </summary>
        public static ReadOnlyObservableCollection<TargetType> CommandTargets { get; set; }

        /// <summary>
        ///     Sets the ability field.
        /// </summary>
        public void AutoFill()
        {
            // Return if string null or empty. 
            if (string.IsNullOrWhiteSpace(Name)) return;

            // Get the ability by name. 
            var ability = FindAbility(Name);

            // Attempt to set the ability and inform the 
            // user of its sucess. 
            if (ability == null)
            {
                AppInformer.InformUser("Auto-Fill failed to find {0} in resources. ", Name);
            }
            else
            {
                Ability = ability;
                AppInformer.InformUser("Auto-Filling for {0} complete. ", Name);

                // Manually signal AbilityType that a change has occured. 
                OnPropertyChanged("AbilityType");
            }
        }

        /// <summary>
        ///     Locates an ability by name and prompting the user if
        ///     more than one ability has been found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Ability FindAbility(string name)
        {
            // Retriever all moves with the specified name. 
            var moves = App.AbilityService.GetAbilitiesWithName(name).ToArray();

            // Prompt user to select a move if more 
            // than one are found with the same name. 
            // Otherwise, return the first occurence or null. 
            if (moves.Length > 1)
            {
                return new AbilitySelectionBox(name).SelectedAbility;
            }

            return moves.FirstOrDefault();
        }
    }
}