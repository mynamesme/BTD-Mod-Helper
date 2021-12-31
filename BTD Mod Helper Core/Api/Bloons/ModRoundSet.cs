﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.Rounds;
using Assets.Scripts.Unity;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Extensions;
using UnhollowerBaseLib;

namespace BTD_Mod_Helper.Api.Bloons
{
    /// <summary>
    /// Class for a custom RoundSet
    /// </summary>
    public abstract class ModRoundSet : NamedModContent
    {
        protected override float RegistrationPriority => 9;

        /// <inheritdoc />
        public override void Register()
        {
            model = GetDefaultRoundSetModel();

            for (var i = 0; i < model.rounds.Count; i++)
            {
                ModifyRoundModels(model.rounds[i], i);

                if (i <= 40)
                {
                    ModifyEasyRoundModels(model.rounds[i], i);
                }
                else if (i <= 60)
                {
                    ModifyMediumRoundModels(model.rounds[i], i);
                }
                else if (i <= 80)
                {
                    ModifyHardRoundModels(model.rounds[i], i);
                }
                else if (i <= 100)
                {
                    ModifyImpoppableRoundModels(model.rounds[i], i);
                }
            }

            Game.instance.model.roundSets = Game.instance.model.roundSets.AddTo(model);
            Game.instance.model.roundSetsByName[Id] = model;
            Game.instance.model.AddChildDependant(model);
        }

        /// <inheritdoc />
        public override void RegisterText(Il2CppSystem.Collections.Generic.Dictionary<string, string> textTable)
        {
            base.RegisterText(textTable);

            if (CustomHints)
            {
                for (var round = 0; round < DefinedRounds; round++)
                {
                    var hint = GetHint(round);
                    if (hint != null)
                    {
                        textTable.Add(HintKey(round), hint);
                    }
                }
            }
        }

        /// <inheritdoc />
        public sealed override string DisplayNamePlural => base.DisplayNamePlural;

        /// <summary>
        /// The id of the existing RoundSet to use as a base. Use RoundSetType.[name]
        /// If this RoundSetType.None, empty, or null, then an empty round set will be used
        /// </summary>
        public abstract string BaseRoundSet { get; }

        /// <summary>
        /// The total number of rounds that have specified Bloons.
        /// <br/>
        /// After this number of rounds, randomized Free Play rounds will happen
        /// </summary>
        public abstract int DefinedRounds { get; }

        /// <summary>
        /// The Base Rounds included in the RoundSet specified by BaseRoundSet
        /// </summary>
        protected List<RoundModel> BaseRounds =>
            Game.instance.model.roundSets.FirstOrDefault(set => set.name == BaseRoundSet)?.rounds.ToList() ??
            new List<RoundModel>();
        
        /// <summary>
        /// The Icon for the Button for this RoundSet within the UI, by default looking for the same name as the file
        /// </summary>
        public virtual string Icon => GetType().Name;

        /// <summary>
        /// If you're not going to use a custom .png for your Icon, use this to directly control its SpriteReference
        /// </summary>
        public virtual SpriteReference IconReference => GetSpriteReference(Icon);

        /// <summary>
        /// Whether this Round set should show up in the menu allowing you to use any RoundSet for any GameMode
        /// </summary>
        public virtual bool AddToOverrideMenu => true;

        /// <summary>
        /// Called to modify any/all rounds from 1 to <see cref="DefinedRounds"/>
        /// </summary>
        public virtual void ModifyRoundModels(RoundModel roundModel, int round)
        {
        }

        /// <summary>
        /// Called to modify specifically just rounds from 1 to 40
        /// </summary>
        public virtual void ModifyEasyRoundModels(RoundModel roundModel, int round)
        {
        }

        /// <summary>
        /// Called to modify specifically just rounds from 41 to 60
        /// </summary>
        public virtual void ModifyMediumRoundModels(RoundModel roundModel, int round)
        {
        }

        /// <summary>
        /// Called to modify specifically just rounds from 61 to 80
        /// </summary>
        public virtual void ModifyHardRoundModels(RoundModel roundModel, int round)
        {
        }

        /// <summary>
        /// Called to modify specifically just rounds from 81 to 100
        /// </summary>
        public virtual void ModifyImpoppableRoundModels(RoundModel roundModel, int round)
        {
        }

        /// <summary>
        /// Whether these rounds should have custom hints, like Alternate Bloons Rounds does
        /// </summary>
        public virtual bool CustomHints => false;

        /// <summary>
        /// Gets the custom hint for a specific round. Make sure <see cref="CustomHints"/> is overriden as true.
        /// <br/>
        /// For no hint, return null.
        /// </summary>
        public virtual string GetHint(int round)
        {
            return null;
        }

        internal string HintKey(int round) => $"{Id} Hint {round}";

        internal RoundSetModel model;

        internal RoundSetModel GetDefaultRoundSetModel()
        {
            var roundSetModel = new RoundSetModel(Id, new Il2CppReferenceArray<RoundModel>(DefinedRounds));

            for (var i = 0; i < DefinedRounds; i++)
            {
                roundSetModel.rounds[i] = i < BaseRounds.Count
                    ? BaseRounds[i].Duplicate()
                    : new RoundModel("RoundModel_", new Il2CppReferenceArray<BloonGroupModel>(0));
                roundSetModel.rounds[i].emissions_ = null;
            }

            return roundSetModel;
        }
    }
}