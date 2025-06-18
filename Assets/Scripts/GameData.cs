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

        // path to the xml file which save player data 
        [SerializeField] private string path = "GameData"; // GameData

        // RECORDS VARIABLES
        private Dictionary<string, StepRecords> _stepRecords;
        private Dictionary<string, PatientCaseRecords> _patientCaseRecords;
        private Dictionary<LevelState, LevelRecords> _levelRecords;
        private MainData _MainData;
        private string _currentKey;
        
        // TIMER VARIABLES
        private TimerData _levelTimer;
        private TimerData _globalTimer;


        /// <summary>
        /// Initializes the record collections used to store level, patient case, and step data.
        /// </summary>
        /// <remarks>This method sets up the necessary dictionaries for managing records. If the level
        /// records collection is null, it will be initialized. Patient case and step records collections are always
        /// reinitialized.</remarks>
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
        /// Updates the internal step records with the specified algorithm step.
        /// </summary>
        /// <remarks>This method associates the provided algorithm step with a new set of step records.
        /// Existing records for the same key will be overwritten.</remarks>
        /// <param name="algoStep">The algorithm step to be recorded. This value is used to generate a unique key for storing the step records.</param>
        public void SetStepRecords(Step algoStep)
        {
            _currentKey = GetUniqueKeyStep(_stepRecords, algoStep.ToString());
            _stepRecords[_currentKey] = new StepRecords(0, 0, new List<string>(), new List<string>());
        }

        
        /// <summary>
        /// Records the progress and results of a step in the algorithm, updating diagnostic and action attempts.
        /// </summary>
        /// <remarks>This method updates the diagnostic and action attempts for the specified step, and
        /// determines whether the step is considered successful. A step is marked as successful if it has at least one
        /// action attempt, regardless of diagnostic attempts. If <paramref name="isDiagnotics"/> is <see
        /// langword="true"/>, the diagnostic attempt count is incremented, and the provided answer is added to the
        /// diagnostic answers. If <paramref name="isAction"/> is <see langword="true"/>, the action attempt count is
        /// incremented, and the provided answer is added to the action answers.</remarks>
        /// <param name="algoStep">The step of the algorithm being recorded.</param>
        /// <param name="isDiagnotics">Indicates whether the step is a diagnostic step. <see langword="true"/> if it is; otherwise, <see
        /// langword="false"/>.</param>
        /// <param name="isAction">Indicates whether the step is an action step. <see langword="true"/> if it is; otherwise, <see
        /// langword="false"/>.</param>
        /// <param name="answer">The answer or result associated with the step. Can be <see langword="null"/> or empty if no answer is
        /// provided.</param>
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

        /// <summary>
        /// Generates a unique key for a step by appending an incrementing index to the base name.
        /// Ensures that the generated key does not already exist in the provided dictionary.
        /// </summary>
        /// <param name="dict">The dictionary to check for existing keys.</param>
        /// <param name="baseName">The base name to use for the key.</param>
        /// <returns>A unique key string based on the base name and index.</returns>
        /// </summary>
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
        
        /// <summary>
        /// Associates a new <see cref="PatientCaseRecords"/> instance with the specified patient name if one does not
        /// already exist.
        /// </summary>
        /// <remarks>If the specified <paramref name="patientName"/> is not already present in the
        /// collection,  a new <see cref="PatientCaseRecords"/> instance is created and added. If the patient name
        /// already exists, no changes are made.</remarks>
        /// <param name="patientName">The name of the patient for whom the case records are being set. Cannot be null or empty.</param>
        public void SetPatientCaseRecorder(string patientName)
        {
            if (!_patientCaseRecords.ContainsKey(patientName)) _patientCaseRecords[patientName] = new PatientCaseRecords();
        }

        public void ClearPatientCaseRecordsOnNextLevel()
        {
            _patientCaseRecords.Clear();
        }

        /// <summary>
        /// Retrieves the case records for a specified patient.
        /// </summary>
        /// <param name="name">The name of the patient whose case records are to be retrieved. Cannot be null or empty.</param>
        /// <returns>The <see cref="PatientCaseRecords"/> object containing the case records for the specified patient. If the
        /// patient does not exist, the method may log an error and return an undefined value.</returns>
        public PatientCaseRecords GetPatientCaseRecords(string name)
        {
            Debug.Log("HERE");
            if (!_patientCaseRecords.ContainsKey(name))
            {
                Debug.LogError($"Patient '{name}' not found.");
            }
                return _patientCaseRecords[name];
        }

        /// <summary>
        /// Records the results of a patient's case, updating diagnostic, action, and step statistics.
        /// </summary>
        /// <remarks>This method updates various statistics for the specified patient's case, including
        /// the number of  correct and incorrect diagnostics, actions, and steps, as well as the total time elapsed.  If
        /// the specified <paramref name="patientName"/> does not exist in the patient case records,  an error is
        /// logged.</remarks>
        /// <param name="patientName">The name of the patient whose case results are being recorded.  Must correspond to an existing key in the
        /// patient case records.</param>
        public void RecordsPatientCase(string patientName)
        {
            if (!_patientCaseRecords.ContainsKey(patientName)) Debug.LogError($"Key {patientName} not found in patientCaseRecorder.");

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
            }

            patientCaseRecords.totDiagnosticCorrect += patientCaseRecords.numberDiagCorrect;
            patientCaseRecords.totDiagnosticError += patientCaseRecords.numberDiagIncorrect;

            patientCaseRecords.totActionCorrect += patientCaseRecords.numberActionCorrect;
            patientCaseRecords.totActionError += patientCaseRecords.numberActionIncorrect;

            patientCaseRecords.totStepSucceed += patientCaseRecords.numberStepSucceed;
            patientCaseRecords.totStepFailed += patientCaseRecords.numberStepFailed;

            patientCaseRecords.timePassed = Time.time - _levelTimer.startTime;
            patientCaseRecords.stepRecords = _stepRecords;
            _patientCaseRecords[patientName] = patientCaseRecords;
        }

        /// <summary>
        /// Updates the level records for the specified level state.
        /// </summary>
        /// <remarks>If the specified <paramref name="levelState"/> does not already exist in the level
        /// records, a new entry is created with default values.</remarks>
        /// <param name="levelState">The state of the level for which records should be updated.</param>
        public void SetLevelRecords(LevelState levelState)
        {
            _levelTimer = new TimerData(Time.time);
            if (!_levelRecords.ContainsKey(levelState)) _levelRecords[levelState] = new LevelRecords(0, 0, _patientCaseRecords);
        }


        /// <summary>
        /// Updates the record for the specified level state with the latest attempt count,  total number of completed
        /// patient cases, and associated patient case records.
        /// </summary>
        /// <remarks>If the specified <paramref name="levelState"/> does not exist in the level records
        /// dictionary,  the method performs no action.</remarks>
        /// <param name="levelState">The state of the level to update. Must be a valid key in the level records dictionary.</param>
        public void RecordsLevel(LevelState levelState)
        {
            if (!_levelRecords.ContainsKey(levelState)) return;

            LevelRecords levelRecords = _levelRecords[levelState];
            levelRecords.levelNbAttempt++;
            levelRecords.totPatientCompleted = _patientCaseRecords.Count; // Need to ba change (check if patient is completed)

            levelRecords.patientCaseRecords = _patientCaseRecords;
            
            _levelRecords[levelState] = levelRecords;
        }


        /// <summary>
        /// Converts the case records of a specified patient into an array of string representations.
        /// </summary>
        /// <remarks>The method retrieves data from the internal patient case records and formats it into
        /// string representations. Ensure that the <paramref name="patientName"/> corresponds to a valid entry in the
        /// records.</remarks>
        /// <param name="patientName">The name of the patient whose case records are to be converted. Cannot be null or empty.</param>
        /// <returns>An array of strings containing the patient's case record data, including the number of attempts, total
        /// action errors, total diagnostic errors, and the time passed in hours, minutes, and seconds. Returns <see
        /// langword="null"/> if the specified patient does not exist in the records.</returns>
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

        /// <summary>
        /// Retrieves step records as a dictionary of string keys and string array values,  representing diagnostic and
        /// action attempts, answers, and success status for a specific step.
        /// </summary>
        /// <remarks>The returned dictionary provides a structured representation of the step's data,
        /// which can be used for  logging, debugging, or further processing. Ensure that the <paramref
        /// name="indexStep"/> corresponds to  a valid step to avoid exceptions.</remarks>
        /// <param name="indexStep">The index of the step to retrieve records for. Must correspond to a valid step.</param>
        /// <returns>A dictionary where the key is a unique string identifier for the step, and the value is an array of strings 
        /// containing diagnostic attempts, action attempts, the last action answer, the last diagnostic answer,  and a
        /// success status message.</returns>
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
        /// Updates the main records at the start of a new game level.
        /// </summary>
        /// <remarks>This method increments the number of game sessions and updates the total games
        /// played. If the session time queue is null, it initializes it as an empty queue.</remarks>
        public void UpdateMainRecordsOnLevelStart()
        {
            _MainData.nbGameSession++;
            _MainData.totGames = _MainData.totGames + _MainData.nbGameSession;
            
            if (_MainData.sessionTimeQueue == null) _MainData.sessionTimeQueue = new Queue<float>();
        }

        /// <summary>
        /// Updates the main game records at the end of a level.
        /// </summary>
        /// <remarks>This method updates the total game time, session time queue, and level records. It
        /// ensures that the session time queue contains no more than 10 entries, representing the most recent session
        /// durations. Additionally, the updated game data is saved to an XML file.</remarks>
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

        /// <summary>
        /// Converts a floating-point time value, representing seconds, into a formatted string in the "HH:mm:ss"
        /// format.
        /// </summary>
        /// <param name="time">The time value in seconds as a floating-point number. Must be non-negative.</param>
        /// <returns>A string representing the time in "HH:mm:ss" format, where "HH" is hours, "mm" is minutes, and "ss" is
        /// seconds.</returns>
        private static string FloatToHMS(float time)
        {
            int totalSeconds = Mathf.RoundToInt(time);
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds / 60) / 60;
            int seconds = totalSeconds % 60;
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        /// <summary>
        /// Initializes the game session by setting up timers, loading game data, and preparing records.
        /// </summary>
        /// <remarks>This method performs the following actions: <list type="bullet"> <item>Starts the
        /// global timer for tracking session time.</item> <item>Initializes game records and paths for data
        /// storage.</item> <item>Attempts to load the last session's game data from a file, if it exists.</item>
        /// <item>If game data is successfully loaded, resets the session count and prepares level and patient case
        /// records.</item> <item>If no game data file is found, initializes a new game data object.</item>
        /// </list></remarks>
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
