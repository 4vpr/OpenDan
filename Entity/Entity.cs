using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace openDan

{
    abstract class Entity
    {
        public Pos pos = new Pos(0,0);
        public void Draw(float delta)
        {

        }
        public void Update(float delta)
        {

        }
    }
}