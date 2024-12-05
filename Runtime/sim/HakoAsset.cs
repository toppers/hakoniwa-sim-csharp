using System;
using System.Collections.Generic;
using hakoniwa.pdu.interfaces;
using hakoniwa.sim.core.impl;
using UnityEngine;

namespace hakoniwa.sim.core
{
    public class HakoAsset: MonoBehaviour, IHakoPdu, IHakoControl
    {
        private static HakoAsset Instance { get; set; }

        [SerializeField]
        private string assetName = "UnityAsset";
        [SerializeField]
        private string pduConfigPath;

        [SerializeField]
        private GameObject[] hakoObjects;

        public SimulationState _state;
        public long _worldTime;
        public SimulationState State => _state;
        public long WorldTime => _worldTime;

        private bool isReady = false;
        
        private IHakoAsset hakoAsset;
        private IHakoCommand hakoCommand;

        private void Awake()
        {
            // シングルトンのセットアップ
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // 他のインスタンスが存在する場合は破棄
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されない
        }
        public static IHakoPdu GetHakoPdu()
        {
            return Instance;
        }
        public static IHakoControl GetHakoControl()
        {
            return Instance;
        }
        private bool HakoAssetIsValid(List<IHakoObject> hakoObectList)
        {
            foreach (var obj in hakoObjects)
            {
                IHakoObject ihako = obj.GetComponentInChildren<IHakoObject>();
                if (ihako == null)
                {
                    throw new ArgumentException("Can not find IHakoObject on " + obj.name);
                }
                hakoObectList.Add(ihako);
            }
            if (hakoObectList.Count == 0)
            {
                throw new ArgumentException("IHakoObect is empty...");
            }

            return true;
        }


        async void Start()
        {
            var hakoObectList = new List<IHakoObject>();
            if (!HakoAssetIsValid(hakoObectList))
            {
                Debug.LogError("Invalid HakoAssets");
                return;
            }

            long delta_time = (long)Math.Round((double)Time.fixedDeltaTime * 1000000.0f);

            hakoAsset = new HakoAssetImpl(assetName, delta_time, pduConfigPath);
            hakoCommand = (IHakoCommand)hakoAsset;
            if (hakoAsset.Initialize(hakoObectList))
            {
                Debug.Log("OK: Initialize Hakoniwa");
                bool ret = await hakoAsset.RegisterOnHakoniwa();
                if (ret)
                {
                    Debug.Log("OK: Register on Hakoniwa: " + assetName);
                    isReady = true;
                    Physics.simulationMode = SimulationMode.Script;
                }
                else
                {
                    Debug.LogError("Can not register on Hakoniwa: " + assetName);
                }
                foreach (var ihako in hakoObectList)
                {
                    ihako.EventInitialize();
                }
            }
            else
            {
                Debug.LogError("Can not Initialize Hakoniwa: " + assetName);
            }
        }

        void FixedUpdate()
        {
            if (isReady == false)
            {
                return;
            }
            _state = hakoCommand.GetState();
            _worldTime = hakoCommand.GetWorldTime();
            if (hakoAsset.Execute())
            {
                Physics.Simulate(Time.fixedDeltaTime);
            }
            else
            {
                Debug.Log("Can not execute simulation: " + hakoCommand.GetState());
            }
        }
        async void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            if (hakoAsset != null && isReady)
            {
                bool ret = await hakoAsset.UnRegisterOnHakoniwa();
                isReady = false;
                Debug.Log($"OK: Unregister from Hakoniwa: {assetName} ret: {ret}");
            }
        }

        public IPduManager GetPduManager()
        {
            return hakoAsset.GetPduManager();
        }

        public bool DeclarePduForWrite(string robotName, string pduName)
        {
            var srv = hakoAsset.GetHakoCommunicationService();
            if (srv == null)
            {
                return false;
            }
            int channel_id = GetPduManager().GetChannelId(robotName, pduName);
            int pdu_size = GetPduManager().GetPduSize(robotName, pduName);
            return srv.DeclarePduForWrite(robotName, pduName, channel_id, pdu_size);
        }

        public bool DeclarePduForRead(string robotName, string pduName)
        {
            var srv = hakoAsset.GetHakoCommunicationService();
            if (srv == null)
            {
                return false;
            }
            int channel_id = GetPduManager().GetChannelId(robotName, pduName);
            int pdu_size = GetPduManager().GetPduSize(robotName, pduName);
            return srv.DeclarePduForRead(robotName, pduName, channel_id, pdu_size);
        }

        public long GetWorldTime()
        {
            return hakoCommand.GetWorldTime();
        }

        public HakoSimState GetState()
        {
            SimulationState state = hakoCommand.GetState();
            switch (state)
            {
                case SimulationState.Stopped:
                    return HakoSimState.Stopped;
                case SimulationState.Runnable:
                    return HakoSimState.Runnable;
                case SimulationState.Running:
                    return HakoSimState.Running;
                case SimulationState.Stopping:
                    return HakoSimState.Stopping;
                case SimulationState.Terminated:
                    return HakoSimState.Terminated;
                default:
                    return HakoSimState.Terminated;
            }
        }

        public bool SimulationStart()
        {
            return hakoCommand.SimulationStart();
        }

        public bool SimulationStop()
        {
            return hakoCommand.SimulationStop();
        }

        public bool SimulationReset()
        {
            return hakoCommand.SimulationReset();
        }
    }
}
