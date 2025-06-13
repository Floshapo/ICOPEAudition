using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
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
                SetLevel(currentLevel, currInstance.LevelsData.patientByLevel[(int)currentLevel]);
                // Set Patient case
                SetPatientCase(currentPatientCase, currInstance.LevelsData.patientByLevel[(int)currentLevel].patientsCase[(int)currentPatientCase]);
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

        public void SetLevel(LevelState levelState, PatientCaseData patientData)
        {
            currentLevel = levelState;
            patientCaseData = patientData;

            GameManager.Instance.GameData.SetLevelRecords(currentLevel);

            Debug.Log($"Current Level : {currentLevel}, {patientCaseData.levelName}");
        }

        public int GetCurrentLevel() { return (int) currentLevel; }

        public void SetPatientCase(PatientCase patientCase, NewPatientData newPatient)
        {
            currentPatientCase = patientCase;
            patientData = newPatient;

            GameManager.Instance.GameData.SetPatientCaseRecorder(patientData.fisrtName);
            
            Debug.Log($"Current Patient: {currentPatientCase}, {patientData.surname}");
        }

        public int GetCurrentPatientCase() { return (int)currentPatientCase; }

        public void SetStep(Step step)
        {
            currentStep = step;
            Debug.Log($"Algo Test G: {currentStep}");
            
            GameManager.Instance.GameData.SetStepRecords(currentStep);

            GameManager.Instance.LoadStep(currentStep);
        }

        public void NextLevel()
        {           
            if ((int)currentLevel < LevelsData.patientByLevel.Count)
            {
                SetLevel(currentLevel, patientCaseData);
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
                currentLevel++;
                NextLevel();
            }
        }

        public void NextStep(AlgoStep step)
        {
            currentStep = step.type;
            Debug.Log("Next Level: " + currentStep);
            SetStep(currentStep);
        }

        public void SaveShowScores()
        {
            Debug.Log("Level completed ! ");
            // Saving player data
            SavePlayerData();
            // Load resume screen - Same
            GameManager.Instance.LoadScore(patientData.fisrtName);
        }


        private void SavePlayerData()
        {
            GameManager.Instance.GameData.RecordsPatientCase(patientData.fisrtName);
            GameManager.Instance.GameData.RecordsLevel(currentLevel);
            GameManager.Instance.GameData.UpdateMainRecordsOnLevelEnd();
        }

        // CALL FORM 'PatientScoreManager' BY 'GoToMenu' FUNCTION
        private static void ReturnToGameMenu()
        {
            //return Game menu selection patient
            GameManager.Instance.LoadGameMenu();
            GameManager.Instance.AudioManager.PlayBGM("skyline");
        }

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
                }
            }

            Debug.Log($"Last level played : {currentLevel}, last patient played: {currentPatientCase}");
        } 

        private void Start()
        {
            LevelsData = GameManager.Instance.LevelsData;

            // Set by default current level and current patient case (change later if player has a save)
            currentLevel = LevelState.LEVEL_0;
            currentPatientCase = PatientCase.PATIENT_0;
        }
    }
}