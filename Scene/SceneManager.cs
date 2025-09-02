namespace OpenDan.Scene
{
    // Base scene with simple lifecycle hooks
    public abstract class Scene
    {
        // Called once when this scene becomes active
        public virtual void OnLoad() { }

        // Called once when this scene is replaced or the app quits
        public virtual void OnUnload() { }

        // Fixed update tick (dt in seconds)
        public virtual void Update(double dt) { }

        // Draw the scene 
        public virtual void Draw(){ }
    }

    // Minimal scene manager that holds the active scene
    public sealed class SceneManager
    {
        public Scene? Current { get; private set; }

        public void Set(Scene next)
        {
            if (ReferenceEquals(Current, next)) return;
            Current?.OnUnload();
            Current = next;
            Current?.OnLoad();
        }
        public void Update(double dt) => Current?.Update(dt);
        public void Draw(double alpha) => Current?.Draw();
    }
}
