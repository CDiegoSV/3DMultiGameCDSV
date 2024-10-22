using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dante.Game;

namespace Dante.TTTGame
{
    #region Enums

    public enum TTTGameStates
    {
        //Starting Game
        Sorting_Roles,
        //During Game
        Game_Executing,
        //Finishing Game
        Victory
    }

    #endregion

    public class TTTGameManager : GeneralGameManager
    {


        #region References



        #endregion

        #region Runtime Variables

        protected TTTGameStates _TTTgameState;

        #endregion

        #region Unity Methods

        private void FixedUpdate()
        {
            switch (_TTTgameState)
            {
                case TTTGameStates.Sorting_Roles:
                    ManageSortingRolesState();
                    break;
            }
        }

        #endregion

        #region Local Methods

        #region TTT_FSM_Methods

        #region Sorting Roles
        protected void InitializeSortingRolesState()
        {
            
        }

        protected void ManageSortingRolesState() 
        { 
            
        }

        protected void ExitSortingRolesState()
        {

        }

        #endregion

        #region Game_Executing

        protected void InitializeGame_ExecutingState()
        {

        }

        protected void ManageGame_ExecutingState()
        {

        }

        protected void ExitGame_ExecutingState()
        {

        }

        #endregion

        #region Victory

        protected void InitializeVictoryState()
        {

        }

        protected void ManageVictoryState()
        {

        }

        protected void ExitVictoryState()
        {

        }

        #endregion

        #endregion

        #endregion

        #region Getters Setters



        #endregion

    }
}

