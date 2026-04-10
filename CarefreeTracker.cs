using System.Collections;
using System.Reflection;
using Modding;
using UnityEngine;

namespace CarefreeTracker
{
    public class CarefreeTracker : Mod
    {
        public static CarefreeTracker Instance;

        private static readonly float[] Percentages = { 0f, 10.1f, 20.2f, 30.3f, 50.5f, 70.7f, 80.8f, 90.9f };

        private static readonly FieldInfo HitsField = typeof(HeroController)
            .GetField("hitsSinceShielded", BindingFlags.NonPublic | BindingFlags.Instance);

        public CarefreeTracker() : base("Carefree Tracker") => Instance = this;

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public override void Initialize()
        {
            Log("=== CarefreeTracker Initialize ===");

            if (HitsField == null)
                LogError("ERROR: No se encontro hitsSinceShielded!");
            else
                Log("OK: hitsSinceShielded encontrado");

            On.HeroController.Awake += OnHeroAwake;
            ModHooks.AfterTakeDamageHook += OnAfterTakeDamage;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
        }

        private void OnHeroAwake(On.HeroController.orig_Awake orig, HeroController self)
        {
            orig(self);
            Log("OnHeroAwake - recreando display");


            HitDisplay.Instance?.DestroyThis();

            HitDisplay.Create();

            UpdateDisplay();
        }

        private int OnAfterTakeDamage(int hazardType, int damageAmount)
        {
            UpdateDisplay();
            return damageAmount;
        }

        private string OnSceneLoad(string sceneName)
        {
            Log($"OnSceneLoad: '{sceneName}'");
            HitDisplay.Instance?.DestroyThis();
            return sceneName;
        }

        public void UpdateDisplay()
        {
            if (HeroController.instance == null) return;

            if (HitDisplay.Instance == null)
            {
                Log("UpdateDisplay: HitDisplay.Instance null, recreando");
                HitDisplay.Create();
            }

            if (HitsField == null)
            {
                HitDisplay.Instance.Show("Carefree Melody\nERROR campo");
                return;
            }

            int hits = (int)HitsField.GetValue(HeroController.instance);
            hits = Mathf.Clamp(hits, 0, Percentages.Length - 1);
            float pct = Percentages[hits];
            string label = pct == 0f ? "0%" : $"{pct:F1}%";

            HitDisplay.Instance.Show($"Jackpot\n{label}");
        }
    }
}