using System;

using Unity.Netcode;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace InterruptingCards.Managers
{
    public class LogManager : NetworkBehaviour
    {
        private readonly NetworkVariable<LogLevel> _toServerLevel = new();

#pragma warning disable RCS1169 // Make field read-only.
        [SerializeField] private LogLevel _logToServerLevel;
#pragma warning restore RCS1169 // Make field read-only.

        private enum LogLevel
        {
            Invalid,
            Trace,
            Debug,
            Info,
            Warn,
            Error,
            Fatal,
        }

        public static LogManager Singleton { get; private set; }

        public void Awake()
        {
            Singleton = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                _toServerLevel.Value = _logToServerLevel;
            }
        }

        public override void OnDestroy()
        {
            Singleton = null;
            base.OnDestroy();
        }

        public void Trace(string message)
        {
            UnityDebug.Log("TRACE: " + message);
            LogToServer(message, LogLevel.Trace);
        }

        public void Debug(string message)
        {
            UnityDebug.Log("DEBUG: " + message);
            LogToServer(message, LogLevel.Debug);
        }

        public void Info(string message)
        {
            UnityDebug.Log("INFO: " + message);
            LogToServer(message, LogLevel.Info);
        }

        public void Warn(string message)
        {
            UnityDebug.LogWarning("WARN: " + message);
            LogToServer(message, LogLevel.Warn);
        }

        public void Error(string message)
        {
            UnityDebug.LogError("ERROR: " + message);
            LogToServer(message, LogLevel.Error);
        }

        public void Fatal(string message)
        {
            UnityDebug.LogError("FATAL: " + message);
            LogToServer(message, LogLevel.Fatal);
        }

        private void LogToServer(string message, LogLevel logLevel)
        {
            if (NetworkManager.IsConnectedClient && !IsServer && logLevel >= _toServerLevel.Value)
            {
                LogToServerRpc(message, logLevel);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void LogToServerRpc(string message, LogLevel logLevel, ServerRpcParams serverRpcParams = default)
        {
            message = $"From client {serverRpcParams.Receive.SenderClientId}: " + message;

            switch (logLevel)
            {
                case LogLevel.Trace:
                    Trace(message);
                    break;
                case LogLevel.Debug:
                    break;
                case LogLevel.Info:
                    Info(message);
                    break;
                case LogLevel.Warn:
                    Warn(message);
                    break;
                case LogLevel.Error:
                    Error(message);
                    break;
                case LogLevel.Fatal:
                    Fatal(message);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
