using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameStateManager : MonoBehaviour
    {

        // ENUM
        private enum GameState { MAIN_MENU, GAME_MENU } // Enums for Main_menu and waiting_room

        public enum LevelState { LEVEL_0, LEVEL_1, LEVEL_2, LEVEL_3, LEVEL_4, LEVEL_5 }
        public enum PatientCase { PATIENT_0, PATIENT_1, PATIENT_2 }


        // VARIABLES
        private GameState gameState;
        private LevelState currentLevel;
        private PatientCase currentPatientCase;
        private Step currentStep;

        private LevelsData LevelsData;
        private PatientCaseData patientCaseData;
        private NewPatientData patientData;

        // MAIN MENU / GAME MENU TRANSITIONS
        /// <summary>
        /// Function call by buttons (play and return Menu)
        /// Basicly switch between MAIN_MENU and GAME_MENU
        /// </summary>
        /// <param name="newState">MainState: MAIN_MENU / GAME_MENU</param>
        private void SetMainState(GameState newState)
        {
            var currInstance = GameManager.Instance;
            gameState = newState;
            Debug.Log($"Main State: {gameState}");
            // LOAD SCENE
            if (gameState == GameState.GAME_MENU)
            {
                currInstance.LoadGameMenu();
                // Set level 
                SetLevel(currentLevel);
                // Set Patient Case
                SetPatientCase(currentPatientCase, patientCaseData.patientsCase[(int)currentPatientCase]);
            }
            else
            {
                currInstance.LoadMainMenu();
            }
        }

        /// <summary>
        /// Public function change the main state between MAIN_MENU & GAME_MENU.
        /// Its call by buttons "Play" & "return menu"
        /// </summary>
        public void ChangeMainState()
        {
            if (gameState == GameState.MAIN_MENU)
            {
                SetMainState(GameState.GAME_MENU);
            }
            else
            {
                SetMainState(GameState.MAIN_MENU);
            }
        }

        /// <summary>
        /// Sets the current level to the given level state,
        /// loads the corresponding patient case data,
        /// and updates the game data with the current level records.
        /// </summary>
        public void SetLevel(LevelState levelState)
        {
            currentLevel = levelState;
            patientCaseData = LevelsData.patientByLevel[(int)levelState];

            GameManager.Instance.GameData.SetLevelRecords(currentLevel);

            Debug.Log($"Current Level : {currentLevel}");
        }

        /// <summary>
        /// Returns the current level as an integer.
        /// </summary>
        public int GetCurrentLevel() { return (int) currentLevel; }

        /// <summary>
        /// Sets the current patient case and loads the associated patient data.
        /// Updates the game data with the current patient's record.
        /// </summary>
        /// <param name="patientCase">The patient case to set as current.</param>
        /// <param name="newPatient">The patient data associated with the current patient case.</param>
        public void SetPatientCase(PatientCase patientCase, NewPatientData newPatient)
        {
            currentPatientCase = patientCase;
            patientData = newPatient;

            GameManager.Instance.GameData.SetPatientCaseRecorder(patientData.firstName);
            
            Debug.Log($"Current Patient: {currentPatientCase}, {patientData.surname}");
        }

        /// <summary>
        /// Returns the current patient case as an integer.
        /// </summary>
        /// <returns>Integer representation of the current patient case.</returns>
        public int GetCurrentPatientCase() { return (int)currentPatientCase; }

        /// <summary>
        /// Sets the current step of the game algorithm.
        /// Updates the game data with the current step records,
        /// then loads the corresponding step content.
        /// </summary>
        /// <param name="step">The step to set as current.</param>
        public void SetStep(Step step)
        {
            currentStep = step;
            Debug.Log($"Algo Test G: {currentStep}");
            
            GameManager.Instance.GameData.SetStepRecords(currentStep);

            GameManager.Instance.LoadStep(currentStep);
        }

        /// <summary>
        /// Advances the game to the next level and patient case if available.
        /// If the current level is less than the total number of levels, it sets the current level and patient case,
        /// then returns to the game menu.
        /// Otherwise, it logs that all levels are completed, resets the game to the first level and patient case,
        /// and returns to the game menu (temporary fix).
        /// </summary>
        public void NextLevel()
        {
            if ((int)currentLevel < LevelsData.patientByLevel.Count)
            {
                SetLevel(currentLevel);
                SetPatientCase(currentPatientCase, patientCaseData.patientsCase[(int)currentPatientCase]);
                //Return to game menu
                ReturnToGameMenu();
            }
            else
            {
                Debug.Log("Tout les niveau sont terminer ! Restart du jeu!");
                // TAMPORARY FIX - Restart the game
                currentLevel = LevelState.LEVEL_0;
                currentPatientCase = PatientCase.PATIENT_0;
                //SET RANDOM MOD (load patient in random make list of all patient)
                //Return to game menu
                ReturnToGameMenu();
            }    
        }

        /// <summary>
        /// Advances to the next patient case within the current level.
        /// If there are more patient cases available, it sets the next patient case and returns to the game menu.
        /// Otherwise, it resets the patient case to the first one, increments the level,
        /// clears patient case records if necessary, and proceeds to the next level.
        /// </summary>
        public void NextPatientCase()
        {
            currentPatientCase++;
            if ((int)currentPatientCase < patientCaseData.patientsCase.Count )
            {
                SetPatientCase(currentPatientCase, patientCaseData.patientsCase[(int)currentPatientCase]);
                ReturnToGameMenu();
            }
            else
            {
                Debug.Log("Tous les cas patient sont terminer! Next Level !");
                currentPatientCase = PatientCase.PATIENT_0; // Reset patient case
                currentLevel++;
                // Clear patient case records                 
                NextLevel();
            }
        }

        /// <summary>
        /// Advances to the next step based on the provided AlgoStep.
        /// Updates the current step and triggers the step setup.
        /// </summary>
        /// <param name="step">The next AlgoStep containing the step type to set.</param>
        public void NextStep(AlgoStep step)
        {
            currentStep = step.type;
            Debug.Log("Next step: " + currentStep);
            SetStep(currentStep);
        }

        /// <summary>
        /// Called when a patient case is completed.
        /// Saves the current player data and loads the score summary screen.
        /// </summary>
        public void SaveShowScores()
        {
            Debug.Log("Patient completed ! ");
            // Saving player data
            SavePlayerData();
            // Load resume screen
            GameManager.Instance.LoadScore(patientData.firstName);
        }

        /// <summary>
        /// Saves the player’s progress related to the current patient case,
        /// updates the level records and main game records at the end of the level.
        /// </summary>
        private void SavePlayerData()
        {
            GameManager.Instance.GameData.RecordsPatientCase(patientData.firstName);
            GameManager.Instance.GameData.RecordsLevel(currentLevel);
            GameManager.Instance.GameData.UpdateMainRecordsOnLevelEnd();
        }

        // CALL FORM 'PatientScoreManager' BY 'GoToMenu' FUNCTION
        /// <summary>
        /// Returns the player to the main game menu (patient selection screen)
        /// and plays the background music "skyline".
        /// </summary>
        private static void ReturnToGameMenu()
        {
            //return Game menu selection patient
            GameManager.Instance.LoadGameMenu();
            GameManager.Instance.AudioManager.PlayBGM("skyline");
        }

        /// <summary>
        /// Loads the saved player state by setting the current level and patient case.
        /// If the saved patient case is not the last in the current level, 
        /// it advances to the next patient case. Otherwise, it moves to the next level.
        /// </summary>
        /// <param name="savedLevelState">The saved level state to load.</param>
        /// <param name="savedPatientCase">The saved patient case to load.</param>
        public void LoadPlayerSaveStates(LevelState savedLevelState, PatientCase savedPatientCase)
        {
            if ((int)savedLevelState <= GameManager.Instance.LevelsData.patientByLevel.Count)
            {
                currentLevel = savedLevelState;

                // WARNING : if cond not good
                if ((int) savedPatientCase < GameManager.Instance.LevelsData.patientByLevel[(int)currentLevel].patientsCase.Count - 1)
                {
                    currentPatientCase = savedPatientCase + 1;
                }
                else
                {
                    // TO CHANGE : Load next level & patientCase = 0
                    currentPatientCase = savedPatientCase;
                    currentLevel++;
                }
            }

            Debug.Log($"Next level played : {currentLevel}, Next patient played: {currentPatientCase}");
        }

        /// <summary>
        /// Initializes the LevelsData reference from the GameManager and sets
        /// the default current level and patient case. These defaults can be
        /// overridden later if a saved game is loaded.
        /// </summary>
        private void Start()
        {
            LevelsData = GameManager.Instance.LevelsData;

            // Set by default current level and current patient case (change later if player has a save)
            currentLevel = LevelState.LEVEL_0;
            currentPatientCase = PatientCase.PATIENT_0;
        }
    }
}