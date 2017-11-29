using Antura.Core;

namespace Antura.Profile
{
    public struct SceneTransition
    {
        public AppScene fromScene;
        public AppScene toScene;
        // public bool keepAsBackable;

        public SceneTransition(AppScene fromScene, AppScene toScene, bool keepAsBackable = false)
        {
            this.fromScene = fromScene;
            this.toScene = toScene;
            //this.keepAsBackable = keepAsBackable;
        }


        public bool Equals(SceneTransition other)
        {
            return fromScene == other.fromScene && toScene == other.toScene;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SceneTransition && Equals((SceneTransition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)fromScene * 397) ^ (int)toScene;
            }
        }

    }

}
