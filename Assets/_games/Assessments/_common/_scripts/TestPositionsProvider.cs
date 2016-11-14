using UnityEngine;

namespace EA4S.Assessment
{
    public class TestPositionsProvider : MonoBehaviour
    {
        void Start()
        {
            var cam = Camera.main;
            float h = cam.orthographicSize * 2;
            float w = h * cam.aspect;
            GenerateCubes( new DefaultPositionsProvider( h, w, Vector3.zero));
        }

        public GameObject questionPrefab;
        public GameObject placeHolderPrefab;
        public GameObject correctPrefab;
        public GameObject wrongPrefab;

        void GenerateCubes( IPositionsProvider provider)
        {
            provider.Reset();
            provider.AddQuestion( 2, 1);
            provider.AddQuestion( 1, 2);
            provider.AddQuestion( 1, 0);
            var positions = provider.GetPositions();

            foreach( var p in positions.Positions)
            {
                foreach (var q in p.QuestionPivots)
                    SpawnCube( questionPrefab, q);

                foreach(var h in p.PlaceholderPivots)
                    SpawnCube( placeHolderPrefab, h);

                foreach (var c in p.CorrectPivots)
                    SpawnCube( correctPrefab, c);

                foreach (var w in p.WrongPivots)
                    SpawnCube( wrongPrefab, w);
            }
        }

        void SpawnCube( GameObject prefab, Position pos)
        {
            Instantiate( prefab, pos.position, pos.rotation);
        }
    }
}
