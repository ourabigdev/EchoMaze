using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.UI;
using Stride.UI.Controls;

namespace EchoMaze.Script
{
    public class Timer : SyncScript
    {
        public static Timer Instance;   

        private float elapsedTime = 0f;
        private TextBlock timerUi;
        private UIComponent uiComponent;

        public override void Start()
        {
            Instance = this; 

            uiComponent = Entity.Get<UIComponent>();
            var page = uiComponent.Page;
            timerUi = page.RootElement.FindVisualChildOfType<TextBlock>("Timer");
        }

        public override void Update()
        {
            elapsedTime += (float)Game.UpdateTime.Elapsed.TotalSeconds;
            timerUi.Text = ((int)elapsedTime).ToString();
        }

        public void ResetTimer()
        {
            elapsedTime = 0f;
        }
    }
}
