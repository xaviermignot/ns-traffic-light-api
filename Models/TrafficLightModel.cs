using System.Linq;

namespace TrafficLight.Api.Models
{
    public class TrafficLightModel
    {
        public LightState GreenLightState { get; set; }
        public LightState AmberLightState { get; set; }
        public LightState RedLightState { get; set; }

        internal void SetLight(LightColor color, LightState state)
        {
            switch (color)
            {
                case LightColor.Green:
                    this.GreenLightState = state;
                    break;
                case LightColor.Amber:
                    this.AmberLightState = state;
                    break;
                case LightColor.Red:
                    this.RedLightState = state;
                    break;
                case LightColor.All:
                    new[] { LightColor.Green, LightColor.Amber, LightColor.Red }.ToList().ForEach(c => SetLight(c, state));
                    break;
            }
        }

        internal void SwitchLight(LightColor color)
        {
            switch (color)
            {
                case LightColor.Green:
                    this.GreenLightState = GetInvertedState(this.GreenLightState);
                    break;
                case LightColor.Amber:
                    this.AmberLightState = GetInvertedState(this.AmberLightState);
                    break;
                case LightColor.Red:
                    this.RedLightState = GetInvertedState(this.RedLightState);
                    break;
                case LightColor.All:
                    new[] { LightColor.Green, LightColor.Amber, LightColor.Red }.ToList().ForEach(c => SwitchLight(c));
                    break;
            }
        }

        private static LightState GetInvertedState(LightState state)
        {
            return state == LightState.On ? LightState.Off : LightState.On;
        }
    }
}
