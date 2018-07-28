// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : AiDesigner
// Created          : 06-20-2016
// Modified         : 05-21-2018
// ***********************************************************************
using AiUnity.NLog.Core;
using System;
using UnityEngine;

namespace AiUnity.NLog.Examples.Scripts
{
    /// <summary>
    /// This file attaches to a GameObject and demonstrates the simplicity of creating NLog messages.
    /// The procedure involves getting a logger instance from NLogManger and then writing your log statements.
    /// By using these log messages in your code you gain immense power through the NLog XML configuration.
    /// This configuration is maintained by the comprehensive Unity Editor GUI Window located at
    /// Tools:AiUnity:NLog:Editor.  It provides the ability to route your messages to destinations like
    /// Unity Console, Remote Viewer, File, and in-game Log Console.  In addition you have dynamic control of the
    /// content and format of the NLog messages.  Please see the detailed documentation to learn more about NLog.
    /// 
    /// To see log messages please configure NLog GUI with the desired log level and message target/rule.
    /// </summary>
    [AddComponentMenu("AiUnity/NLog/Examples/NLogExample")]
    public class NLogExample : MonoBehaviour
    {
        private NLogger logger;
        public GameObject gameObjectContext;
        //private int updateCount = 0;

        /// <summary>
        /// Awake called by Unity.
        /// </summary>
        void Awake()
        {
            Debug.Log("Standard Unity Debug log message, called from NLogExample Awake() method.\nPlease configure NLog GUI with the desired rule, target, and message level.");

            // Create class logger and associate it with this class instance/name.
            this.logger = NLogManager.Instance.GetLogger(this);
            // Debug message that can be augmented, filtered, and routed by NLog GUI.
            this.logger.Info("Testing a NLog <i>Info</i> message from {0} Awake() method.", GetType().Name);
        }

        /// <summary>
        /// Start called by Unity.
        /// </summary>
        /// <exception cref="Exception">Test Exception</exception>
        void Start()
        {
            // Basic logging statements
            this.logger.Assert(false, "Testing a NLog <i>Assert</i> message from {0} Start() method.", GetType().Name);
            this.logger.Fatal("Testing a NLog <i>Fatal</i> message from {0} Start() method.", GetType().Name);
            this.logger.Error("Testing a NLog <i>Error</i> message from {0} Start() method.", GetType().Name);
            this.logger.Warn("Testing a NLog <i>Warn</i> message from {0} Start() method.", GetType().Name);
            this.logger.Info("Testing a NLog <i>Info</i> message from {0} Start() method.", GetType().Name);
            this.logger.Debug("Testing a NLog <i>Debug</i> message from {0} Start() method.", GetType().Name);
            this.logger.Trace("Testing a NLog <i>Trace</i> message from {0} Start() method.", GetType().Name);

            // Perform assertion test
            int assertCondition = 1;
            this.logger.Assert(assertCondition == 0, "Assertion fired because assertCondition={0}.", assertCondition);

            // Use the <context> argument to explicitly associate a message with a gameObject
            this.logger.Info(gameObjectContext, "Test NLog message with an explicit gameObject context");

            // Create a test exception for illustration.
            Exception testException;
            try { throw new Exception("Test Exception"); }
            catch (Exception exception) { testException = exception; }

            // Added exception will be formatted and appended to message
            this.logger.Info(testException, "Test NLog message with an exception");
        }

        private void Update()
        {
            //if (updateCount++ < 100)
            //this.logger.Info("Update message count={0}", updateCount);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label("If messages missing on expected destination please configure NLog GUI with desired LogLevel (i.e. Everything) and target (i.e. UnityConsole/GameConsole).");
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}