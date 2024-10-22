using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dante.Game
{
    #region Enums

    public enum GameStates { None, Standby, Game, Finish}

    #endregion

    public class GeneralGameManager : MonoBehaviour
    {
        #region Runtime Variables

        protected GameStates _gameState;

        #endregion

        #region Unity Methods

        private void Start()
        {
            InitializeGameManager();
        }

        #endregion

        #region Local Methods

        protected virtual void InitializeGameManager()
        {
            _gameState = GameStates.Standby;
        }

        #endregion
    }
}

