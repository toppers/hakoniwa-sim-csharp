using System;
using System.Collections.Generic;
using hakoniwa.sim.core.impl;
using UnityEngine;

namespace hakoniwa.sim.core
{
    public class HakoAsset: MonoBehaviour
    {
        [SerializeField]
        private string assetName = "UnityAsset";

        [SerializeField]
        private GameObject[] hakoObjects;

        public SimulationState _state;
        public long _worldTime;
        public SimulationState State => _state;
        public long WorldTime => _worldTime;

        private bool isReady = false;
        
        private IHakoAsset hakoAsset;
        private IHakoCommand hakoCommand;

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


        void Start()
        {
            var hakoObectList = new List<IHakoObject>();
            if (!HakoAssetIsValid(hakoObectList))
            {
                Debug.LogError("Invalid HakoAssets");
                return;
            }

            long delta_time = (long)Math.Round((double)Time.fixedDeltaTime * 1000000.0f);

            hakoAsset = new HakoAssetImpl(assetName, delta_time);
            hakoCommand = (IHakoCommand)hakoAsset;
            if (hakoAsset.Initialize(hakoObectList))
            {
                Debug.Log("OK: Initialize Hakoniwa");
                bool ret = hakoAsset.RegisterOnHakoniwa();
                //bool ret = false;
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
        void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");
            if (hakoAsset != null && isReady)
            {
                bool ret = hakoAsset.UnRegisterOnHakoniwa();
                isReady = false;
                Debug.Log($"OK: Unregister from Hakoniwa: {assetName} ret: {ret}");
            }
        }

    }
}
