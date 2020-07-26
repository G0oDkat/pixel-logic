namespace PixelLogic
{
    using Windows;

    internal class PixelLogicApp : Application
    {
        private MainWindow window;

        protected override void OnStart()
        {
            window = CreateWindow(() => new MainWindow(), "PIXEL LOGIC");
            window.Destroyed += OnWindowDestroyed;
            window.CenterToScreen();
            window.Show();
        }

        private void OnWindowDestroyed()
        {
            Exit();
        }
    }
}