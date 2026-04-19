using UnityEngine;

public class Lvl1 : MonoBehaviour
{
    [SerializeField]
    private GameObject _door;

    [SerializeField]
    private GameObject _hinge1;

    [SerializeField]
    private GameObject _hinge2;

    private StackedTileBehaviour _doorTileBehaviour;

    public void Start()
    {
        _doorTileBehaviour = _door.GetComponent<StackedTileBehaviour>();

        GlobalHeartBehaviour.Instance.StateChanged += InstanceOnStateChanged;
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
    
    private void InstanceOnStateChanged(HeartState _)
    {
        if (_door != null)
        {
            _doorTileBehaviour.SetTileAmount(_doorTileBehaviour.TileAmount - 1);

            if (_doorTileBehaviour == null)
            {
                _hinge1.GetComponent<StaticTileBehaviour>().SetTileValue("Corner4");
                _hinge2.GetComponent<StaticTileBehaviour>().SetTileValue("Corner1");

                GlobalHeartBehaviour.Instance.StateChanged -= InstanceOnStateChanged;
                Destroy(_door);
            }
            

            //if (_doorTileBehaviour.TileAmount > 1)
            //{
            //    _doorTileBehaviour.SetTileAmount(_doorTileBehaviour.TileAmount - 1);
            //}
            //else
            //{
            //    _hinge1.GetComponent<StaticTileBehaviour>().SetTileValue("Corner4");
            //    _hinge2.GetComponent<StaticTileBehaviour>().SetTileValue("Corner1");

            //    GlobalHeartBehaviour.Instance.StateChanged -= InstanceOnStateChanged;
            //    Destroy(_door);
            //}
        }
    }
}
