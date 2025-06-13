using Assets.Scripts.Managers;
using Assets.Scripts.PatientData.AlgoData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Managers.GameStateManager;

namespace Assets.Scripts
{
    public class GameData : MonoBehaviour
    {
        // RECORDS OF CURRENT STEPS OF ALGO - DATA TO SHOW IN STEP SELECTOR OR STORE
        public struct StepRecords
        {           
            public int diagnoticsAttempt;
            public int actionAttempt;
            public List<string> diagnosticAnswer; // None, Error description
            public List<string> actionAnswer; // None, Error description
            public bool succeeded; // No error

            public StepRecords(int diagnoticsAttempt, int actionAttempt, List<string> diagnosticError, List<string> actionError)
            {
                this.diagnoticsAttempt = diagnoticsAttempt;
                this.actionAttempt = actionAttempt;
                this.diagnosticAnswer = diagnosticError;
                this.actionAnswer = actionError;
                this.succeeded = false;
            }
        }

        // RECORD OF CURRENT LEVEL - DATA TO SHOW IN LEVEL SELECTOR OR STORE
        public struct PatientCaseRecords
        {
            // TOT Data on the current patient
            public int nbAttempt { get; set; } // Number of attempts for this patient
            public int totDiagnosticCorrect { get; set; }
            public int totDiagnosticError { get; set; } // length of diagnosticError
            public int totActionCorrect { get; set; }
            public int totActionError { get; set; } // length of actionError
            public int totError => totActionError + totDiagnosticError; // totActionError + totDiagnosticError
            public int totStepSucceed { get; set; } // Number of succeeded (count number of succeeded in StepRecord)
            public int totStepFailed { get; set; } // Number of failed (count number of failed in StepRecord)

            // data we want the show for the player for his last attempt
            public int numberDiagCorrect { get; set; }
            public int numberDiagIncorrect { get; set; }
            public int numberActionCorrect { get; set; }
            public int numberActionIncorrect { get; set; }
            public int numberStepSucceed { get; set; }
            public int numberStepFailed { get; set; }
            public float successRate => (numberStepSucceed + numberStepFailed) == 0 ? 0f : (float)numberStepSucceed * 100 / ((float)numberStepSucceed + (float)numberStepFailed);
            
            public float timePassed { get; set; } // Time spent on the level
            public Dictionary<string, StepRecords> stepRecords; // StepRecords of the level {Step name, Steps}
        }

        public struct LevelRecords
        {
            public int levelNbAttempt;
            public int totPatientCompleted;
            // ADD OTHER STAT
            public Dictionary<string, PatientCaseRecords> patientCaseRecords; // {Patient Name,  PatientRecords}

            public LevelRecords(int levelNbAttempt, int PatientCompleted, Dictionary<string, PatientCaseRecords> patientCaseRecords)
            {
                this.levelNbAttempt = levelNbAttempt;
                this.totPatientCompleted = PatientCompleted;
                this.patientCaseRecords = patientCaseRecords;
            }
        }

        // GLOBAL RECORDS
        public struct MainData
        {
            public int totGames; // Number of games played
            public float gameTime;
            public int nbGameSession;
            public Queue<float> sessionTimeQueue; 

            public Dictionary<LevelState, LevelRecords> levelRecords; // {levelName, levelRecords}

            public MainData(int nbGames, int nbGameSession, float gameTime, Queue<float> sessionTimeQueue, Dictionary<LevelState, LevelRecords> levelRecords)
            {
                this.totGames = nbGames;
                this.gameTime = gameTime;
                this.nbGameSession = nbGameSession;
                this.sessionTimeQueue = sessionTimeQueue;
                this.levelRecords = levelRecords;
            }
        }

        

        // TIMER DATA
        public struct TimerData
        {
            public float startTime;
            public float elapsedTime;

            public TimerData(float startTime)
            {
                this.startTime = startTime;
                this.elapsedTime = 0f;
            }
        }

        // RECORDS VARIABLES
        private Dictionary<string, StepRecords> _stepRecords;
        private Dictionary<string, PatientCaseRecords> _patientCaseRecords;
        private Dictionary<LevelState, LevelRecords> _levelRecords;
        private MainData _MainData;
        private string _currentKey;
        
        // TIMER VARIABLES
        private TimerData _levelTimer;
        private TimerData _globalTimer;

        [SerializeField] private string path = "GameData"; // GameData

        /// <summary>
        /// Initialize Dictionarys (_levelRecords<LevelState, LevelRecords> & _stepRecords<AlgoState, StepRecords>) when a game start (click on the GrandMa/GrandPa).
        /// </summary>
        public void InitializeRecords()
        {
            if (_levelRecords == null)
            {
                _levelRecords = new Dictionary<LevelState, LevelRecords>();
            }
            _patientCaseRecords = new Dictionary<string, PatientCaseRecords>();
            _stepRecords = new Dictionary<string, StepRecords>();
        }

        /// <summary>
        /// Set the dictionary<AlgoState, StepRecords> _stepRecords as a key an AlgoState (input parameter: currentAlgoState) and a value a new StepRecords.
        /// </summary>
        /// <param name="algoStep"></param>
        public void SetStepRecords(Step algoStep)
        {
            _currentKey = GetUniqueKeyStep(_stepRecords, algoStep.ToString());
            _stepRecords[_currentKey] = new StepRecords(0, 0, new List<string>(), new List<string>());
        }

        /// <summary>
        /// Modify/records the data form the player (attempt, actionError, diagnoticError) of the current step (AlgoState).
        /// Take as inputs AlgoState, string of action error and a string of diagnostic error. 
        /// </summary>
        /// <param name="algoStep"></param>
        /// <param name="action"></param>
        /// <param name="diagnostic"></param>
        public void RecordsSteps(Step algoStep, bool isDiagnotics, bool isAction, string answer)
        {

            // Key : Questionnaire_0 | (string)algoStep+'_'+0 (int)
            if (!_stepRecords.ContainsKey(_currentKey)) return;
            
            StepRecords stepData = _stepRecords[_currentKey];
            
            if (isDiagnotics)
            {
                stepData.diagnoticsAttempt++;
                if (!string.IsNullOrEmpty(answer)) stepData.diagnosticAnswer.Add(answer);
            }
            
            if (isAction)
            {
                stepData.actionAttempt++;
                if (!string.IsNullOrEmpty(answer)) stepData.actionAnswer.Add(answer);
            }

            // Check if has diagnotics or action.
            if (stepData.actionAttempt == 1 && stepData.diagnoticsAttempt == 1) stepData.succeeded = true;
            else if (stepData.actionAttempt == 1 && stepData.diagnoticsAttempt == 0) stepData.succeeded = true;
            else stepData.succeeded = false;

            _stepRecords[_currentKey] = stepData;
        }

        private static string GetUniqueKeyStep(Dictionary<string, StepRecords> dict, string baseName)
        {
            int index = 0;
            string key = "";
            do
            {
                key = baseName + "_" + index;
                index++;
            } while (dict.ContainsKey(key));
            return key;
        }
        
        public void SetPatientCaseRecorder(string patientName)
        {
            if (!_patientCaseRecords.ContainsKey(patientName)) _patientCaseRecords[patientName] = new PatientCaseRecords();
        }

        public PatientCaseRecords GetPatientCaseRecords(string name)
        {
            if (!_patientCaseRecords.ContainsKey(name)) Debug.LogError($"Patient '{name}' not found.");
            return _patientCaseRecords[name];
        }

        public void RecordsPatientCase(string patientName)
        {
            if (!_patientCaseRecords.ContainsKey(patientName)) Debug.LogError("Key "+patientName+" not found in patientCaseRecorder.");

            PatientCaseRecords patientCaseRecords = _patientCaseRecords[patientName];
            patientCaseRecords.nbAttempt++;
            foreach (var step in _stepRecords)
            {
                StepRecords stepData = step.Value;
                if (stepData.diagnosticAnswer.Count > 1) patientCaseRecords.numberDiagIncorrect++;
                else patientCaseRecords.numberDiagCorrect++;

                if (stepData.actionAnswer.Count > 1) patientCaseRecords.numberActionIncorrect++;
                else patientCaseRecords.numberActionCorrect++;

                if (stepData.succeeded) patientCaseRecords.numberStepSucceed++;
                else patientCaseRecords.numberStepFailed++;

                patientCaseRecords.totDiagnosticCorrect += patientCaseRecords.numberDiagCorrect;
                patientCaseRecords.totDiagnosticError += patientCaseRecords.numberDiagIncorrect;

                patientCaseRecords.totActionCorrect += patientCaseRecords.numberActionCorrect;
                patientCaseRecords.totActionError += patientCaseRecords.numberActionIncorrect;

                patientCaseRecords.totStepSucceed += patientCaseRecords.numberStepSucceed;
                patientCaseRecords.totStepFailed += patientCaseRecords.numberStepFailed;
            }

            patientCaseRecords.timePassed = Time.time - _levelTimer.startTime;
            patientCaseRecords.stepRecords = _stepRecords;

            Debug.Log("PLOP");

            _patientCaseRecords[patientName] = patientCaseRecords;
        }

        public void ResetPatientCase(string patientName)
        {
            var patientCase = _patientCaseRecords[patientName];
            patientCase.numberDiagCorrect = 0;
            patientCase.numberDiagIncorrect = 0;
            patientCase.numberActionCorrect = 0;
            patientCase.numberActionIncorrect = 0;
            patientCase.numberStepSucceed = 0;
            patientCase.numberStepFailed = 0;
        }

        /// <summary>
        /// Set the dictionary<LevelState, LevelRecords> _levelRecords as a key a LevelState (input parameter: currentLevelState) and value a new LevelRecords.
        /// </summary>
        /// <param name="levelState"></param>
        public void SetLevelRecords(LevelState levelState)
        {
            _levelTimer = new TimerData(Time.time);
            if (!_levelRecords.ContainsKey(levelState)) _levelRecords[levelState] = new LevelRecords(0, 0, _patientCaseRecords);
        }


        /// <summary>
        /// Modify/records the data from the player (attempt, nbStepSucced, nbStepFailed, totActionError, totDiagnostics, timeSpentInLevel, stepRecords) of the current level (LevelState).
        /// </summary>
        /// <param name="levelState"></param>
        public void RecordsLevel(LevelState levelState)
        {
            if (!_levelRecords.ContainsKey(levelState)) return;

            LevelRecords levelRecords = _levelRecords[levelState];
            levelRecords.levelNbAttempt++;
            levelRecords.totPatientCompleted = _patientCaseRecords.Count; // Need to ba change (check if patient is completed)

            levelRecords.patientCaseRecords = _patientCaseRecords;
            
            _levelRecords[levelState] = levelRecords;
        }



        public string[] PatientCaseRecordsToString(string patientName)
        {
            string[] texts = new string[4];

            // get data from levelRecords
            if (_patientCaseRecords.ContainsKey(patientName))
            {
                texts[0] = _patientCaseRecords[patientName].nbAttempt.ToString();
                texts[1] = _patientCaseRecords[patientName].totActionError.ToString();
                texts[2] = _patientCaseRecords[patientName].totDiagnosticError.ToString();
                texts[3] = FloatToHMS(_patientCaseRecords[patientName].timePassed);
            }
            else
            {
                texts = null;
            }
            return texts;
        }

        public Dictionary<string, string[]> GetStepRecordsToString(int indexStep)
        {
            Step step = (Step)indexStep;
            
            string key = GetUniqueKeyStep(_stepRecords, step.ToString()); // TODO : Fix this shit 

            Dictionary<string, string[]> stringRecords = new Dictionary<string, string[]>();
            
            string[] dataStep = new string[4];
            dataStep[0] = _stepRecords[key].diagnoticsAttempt.ToString();
            dataStep[1] = _stepRecords[key].actionAttempt.ToString();
            dataStep[2] = _stepRecords[key].actionAnswer.AsEnumerable<string>().Last();
            dataStep[3] = _stepRecords[key].diagnosticAnswer.AsEnumerable<string>().Last();
            dataStep[4] = _stepRecords[key].succeeded ? "No error" : "Error";
            stringRecords.Add(_stepRecords[key].ToString(), dataStep);

            return stringRecords;
        }


        /// <summary>
        /// Change the number of game when the game start.
        /// </summary>
        public void UpdateMainRecordsOnLevelStart()
        {
            _MainData.nbGameSession++;
            _MainData.totGames = _MainData.totGames + _MainData.nbGameSession;
            
            if (_MainData.sessionTimeQueue == null) _MainData.sessionTimeQueue = new Queue<float>();
        }

        /// <summary>
        /// Modify/records the data form the player (nbLevelsCompleted, nbStepsCompleted, globalActionErrors, globalDiagnosticErrors, gameTime, currentSessionTime and levelRecords).
        /// Save-it in xml file "GameData".
        /// </summary>
        public void UpdateMainRecordsOnLevelEnd()
        {

            _MainData.gameTime = Time.time - _globalTimer.startTime + _MainData.gameTime; // Add the time spend on the game (the global time)
            
            // Limit number of time save session to 10
            if (_MainData.sessionTimeQueue.Count < 10)
                _MainData.sessionTimeQueue.Enqueue(Time.time - _globalTimer.startTime); // Add the time spend on the session
            else
            {
                _MainData.sessionTimeQueue.Dequeue();
                _MainData.sessionTimeQueue.Enqueue(Time.time - _globalTimer.startTime); // Add the time spend on the session
            }

            _MainData.levelRecords = _levelRecords;

            XmlManager.SaveToXml(_MainData, Path.Combine(Application.streamingAssetsPath, path), "GameData");
        }

        public bool FisrtGameSession()
        {
            return _MainData.nbGameSession == 0;
        }

        private static string FloatToHMS(float time)
        {
            int totalSeconds = Mathf.RoundToInt(time);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds / 60) / 60;
            int seconds = totalSeconds % 60;
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }


        /// <summary>
        /// Unity fuction. On start set _globalTimer and try to get Data form xml file "GameData" and set _globalData, _levelRecords and _stepRecords with the loaded data.
        /// </summary>
        void Start()
        {
            _globalTimer = new TimerData(Time.time); // Start the global timer            
            InitializeRecords();
            path = Path.Combine(Application.streamingAssetsPath, path);
            // try to get last session time on web request
            if (File.Exists(path))
            {
                
                _MainData = XmlManager.LoadGameData(Path.Combine(Application.streamingAssetsPath, path));
                _MainData.nbGameSession = 0; // Set the number of session game to 0
                _levelRecords = _MainData.levelRecords;

                foreach (var keyValue in _levelRecords)
                {
                    _patientCaseRecords = keyValue.Value.patientCaseRecords;
                }

                LevelState lastLevelPlayed = _MainData.levelRecords.Keys.Last();
                var lastPatientPlayed = _MainData.levelRecords[lastLevelPlayed].patientCaseRecords.Count - 1; // WARNING ...

                GameManager.Instance.GameStateManager.LoadPlayerSaveStates(lastLevelPlayed, (PatientCase)lastPatientPlayed); //WARNING TOO
            }
            else
            {
                _MainData = new MainData();
            }
        }

    }
}


/*
 * 
 * Format that player data will be saved in the database:
    - UID
    - PlayerData
        |  GlobalData
        |  LevelData
        |  StepData   

 XML FORMAT:
    | UID - string
    | PlayerData
    |   | GlobalData - Struct
    |   |   | nbGames - int
    |   |   | nbLevelsCompleted - int
    |   |   | nbStepsCompleted - int
    |   |   | nbActionErrors - int
    |   |   | nbDiagnosticErrors - int
    |   |   | gameTime - TimerData
    |   |   | currentSessionTime - TimerData 
    |   | LevelData - Struct
    |   |   | Level : Level 0 - enum
    |   |   |   | levelAttempt - int
    |   |   |   | totActionError - int
    |   |   |   | totDiagnosticError - int
    |   |   |   | nbStepSucced - int
    |   |   |   | nbStepFailed - int
    |   |   |   | levelTime - TimerData
    |   |   | Level : Level 1 - enum
    |   |   |   | levelAttempt - int
    |   |   |   | totActionError - int
    |   |   |   | totDiagnosticError - int
    |   |   |   | nbStepSucced - int
    |   |   |   | nbStepFailed - int
    |   |   |   | levelTime - TimerData
    |   | StepData - Struct
    |   |   | Step : Questionary - enum
    |   |   |   | attempt - int
    |   |   |   | actionError - List<string>
    |   |   |   | diagnosticError - List<string>
    |   |   | Step : Diagnostic - enum
    |   |   |   | attempt - int
    |   |   |   | actionError - List>string>
    |   |   |   | diagnosticError - List<string>
 
LevelData : Dictonary<LevelState, LevelRecord>
StepData : Dictonary<AlgoState, StepRecords>


    [XML/JSON/...]
      UID - Player ID
      PlayerData 
        |
        | Main Data:
        |   | nbGames 
        |   | nbLevelCompleted
        |   | nbRandomLevelCompleted
        |   | totalGameTime 
        |   | currentSessionTime
        | LevelData
        |   | Level : Level 0
        |   |   | totAttempt - int (nbAttemp per patient case )
        |   |   | totNoErrSucc - int ((totActionErro + totDiagError) per patient = 0 )
        |   |   Patient Case
        |   |      | nbAttempt - int
        |   |      | nbNoErrorSucc - int
        |   |      | totActionError - int
        |   |      | totDiagnoticError - int
        |   |      | timePassed - timer
        |   |   StepData : 
        |   |      | Step : Case_presentation
        |   |      |   | attempt - int
        |   |      |   | diagnosticError - List<string>
        |   |      |   | actionError - List<string>
        |   |      | Step : Wisper_test
        |   |      |   | attempt - int
        |   |      |   | diagnosticError - List<string>
        |   |      |   | actionError - List<string>


 */
