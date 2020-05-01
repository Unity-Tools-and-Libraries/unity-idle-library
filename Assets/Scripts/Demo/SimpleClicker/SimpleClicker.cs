using IdleFramework;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using BreakInfinity;
using IdleFramework.State.Matchers;
using IdleFramework.Configuration;
using IdleFramework.Configuration.UI.Components;
using IdleFramework.Configuration.UI;

public class SimpleClicker : MonoBehaviour
{
    public IdleEngine engine;

    void Awake()
    {

        GameConfigurationBuilder gc = new GameConfigurationBuilder();
        gc.WithEntity(new EntityDefinitionBuilder("clicks"));

        var producersPanelConfiguration = new PanelConfiguration("producers")
            .WithLayout(new VerticalLayoutConfiguration());

        var upgradesPanelConfiguration = new PanelConfiguration("upgrades")
            .WithLayout(new VerticalLayoutConfiguration());

        for (int i = 1; i <= 7; i++)
        {
            string key = "autoclickerx" + i;
            string name = "Auto Clicker X " + i;
            string previousKey = "autoclickerx" + (i - 1);
            EntityDefinitionBuilder defBuilder = new EntityDefinitionBuilder(key)
            .WithType("producer")
            .WithCost("clicks", Literal.Of(BigDouble.Pow(10, (i * 2) - 1)).Times(Literal.Of(1.1).ToPower(new EntityQuantityBoughtReference(key))))
            .WithScaledOutput(i == 1 ? "clicks" : previousKey, Exponent.Of(Literal.Of(2),
                Floor.Of(Logarithm.Of(Literal.Of(10), new EntityNumberPropertyReference(key, "HighestQuantityAchieved"))).Minus(1)))
            .WithName(Literal.Of(name));
            if (i != 1)
            {
                defBuilder.HiddenAndDisabledWhen(new EntityNumberPropertyMatcher(previousKey, "HighestQuantityAchieved", Comparison.LESS_THAN, 10));
            }
            gc.WithEntity(defBuilder);

            var upgradeKey = "doubleclickerx" + i;
            var upgradeName = "Double Clicker " + i;

            gc.WithEntity(
                new EntityDefinitionBuilder(upgradeKey)
                    .WithName(upgradeName)
                    .WithType("upgrade")
                    .WithCost(key, 100)
                    .WithEffect(new EntityNumberPropertyModifierDefinition("autoclickerx" + i, "ScaledOutputs.clicks", EffectType.MULTIPLY, Literal.Of(2), new EntityNumberPropertyMatcher("doubleclickerx" + i, "QuantityBought", Comparison.GREATER_THAN_OR_EQUAL, 1)))
                    .LimitOne()
            );

            producersPanelConfiguration.WithChild(new ButtonConfiguration(key, new FormattedString("{0} - {1}", Literal.Of(name), new EntityQuantityReference(key).AsFormattedString("G3")), new EntityAvailableMatcher(key))
                .OnClick(engine => engine.BuyEntity(engine.GetEntity(key))));

            upgradesPanelConfiguration.WithChild(new ButtonConfiguration(upgradeKey, Literal.Of(upgradeName), new EntityAvailableMatcher(upgradeKey))
                .OnClick(engine => engine.BuyEntity(engine.GetEntity(upgradeKey))));
        }

        gc.WithBeforeEntityBuyHook("*", (entity, engine) => { });

        TabConfiguration mainTab = new TabConfiguration("main");
        mainTab.WithText("Clicks")
            .WithLayout(new HorizontalLayoutConfiguration())
            .WithChild(new PanelConfiguration("clicks")
                .WithLayout(new VerticalLayoutConfiguration())
                .WithChild(new LabelConfiguration("click-text", new FormattedString("{0} clicks", new EntityQuantityReference("clicks").AsFormattedString("G3"))))
                .WithChild(new ButtonConfiguration("click", "Click me!").OnClick(engine => engine.GetEntity("clicks").ChangeQuantity(1))))
            .WithChild(producersPanelConfiguration)
            .WithChild(upgradesPanelConfiguration);



        gc.WithUi(new UiConfiguration()
            .WithTab(mainTab));

        engine = new IdleEngine(gc.Build(), gameObject);

        if (File.Exists(Application.persistentDataPath + "/saved.json"))
        {
            string loaded = File.OpenText(Application.persistentDataPath + "/saved.json").ReadToEnd();
            var snapshot = JsonConvert.DeserializeObject<IdleEngineSnapshot>(loaded);
            engine.LoadFromSnapshot(snapshot);
        }

        InvokeRepeating("Save", 1f, 1f);
    }

    public void Click()
    {
        engine.GetEntity("clicks").ChangeQuantity(1);
    }

    void Update()
    {
        engine.Update(Time.deltaTime);
    }

    void Save()
    {
        IdleEngineSnapshot snapshot = engine.GetSnapshot(engine);

        string serialized = JsonConvert.SerializeObject(snapshot);
        if (!File.Exists(Application.persistentDataPath + "/saved.json"))
        {
            var createdStream = File.Create(Application.persistentDataPath + "/saved.json");
            createdStream.Close();
        }
        File.WriteAllText(Application.persistentDataPath + "/saved.json", serialized);
    }

    private void Reset()
    {
        foreach (var entity in engine.AllEntities)
        {
            entity.Value.SetQuantity(1);
        }
    }

}
