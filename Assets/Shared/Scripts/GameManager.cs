using Cysharp.Threading.Tasks;
using Fighting;
using NaughtyAttributes;
using Player;
using Unity.Cinemachine;
using UnityEngine;
using World;
using Quaternion = UnityEngine.Quaternion;

public enum GameState
{
    Resting,
    Walking,
    Attacking,
    Shopping
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private GameState gameState = GameState.Resting;
    [SerializeField] private Enemy _enemy;
    [Space] [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CinemachineCamera fightingCam;

    [SerializeField] private Tile currentTile;
    private Tile _previousTile;
    private Tile _nextTile;

    public void SetGameState(GameState newGameState)
    {
        gameState = newGameState;
        Debug.Log($"Game State changed to {newGameState}");
    }

    private void Start()
    {
        _previousTile = CreateNewTile(Vector3.left * 10);
        currentTile = CreateNewTile(Vector3.zero);
        _nextTile = CreateNewTile(Vector3.right * 10);

        _enemy = SpawnEnemy(currentTile.enemyPosition.position);
        GoToWalking();
    }

    private async UniTask GoToWalking()
    {
        SetGameState(GameState.Walking);
        targetGroup.Targets.Clear();
        targetGroup.Targets.Add(new CinemachineTargetGroup.Target
        {
            Object = _enemy.transform,
            Radius = 1,
            Weight = 1
        });
        targetGroup.Targets.Add(new CinemachineTargetGroup.Target
        {
            Object = FindAnyObjectByType<Character>().transform,
            Radius = 1,
            Weight = 1
        });
        await character.WalkTo(currentTile.restingPosition.position.x);
        GoToResting();
    }

    private void GoToResting()
    {
        SetGameState(GameState.Resting);
    }

    private async UniTask GoToAttacking()
    {
        SetGameState(GameState.Attacking);

        await character.SlideTo(currentTile.duelPosition.position.x, .8f);

        var attacker = character.Fighter;
        var target = _enemy.Health;

        while (character.Health.IsAlive && _enemy.Health.IsAlive)
        {
            await attacker.Attack(target);

            // switch attacker and target
            attacker = attacker == character.Fighter ? _enemy.Fighter : character.Fighter;
            target = target == character.Health ? _enemy.Health : character.Health;
        }

        if (character.Health.IsDead)
        {
            ResetFight();
        }
        else
        {
            character.Health.HealFully();
            FinishTile();
            await GoToWalking();
        }
    }

    private void FinishTile()
    {
        if (_previousTile)
            Destroy(_previousTile.gameObject);
        _previousTile = currentTile;
        currentTile = _nextTile;
        _nextTile = CreateNewTile(currentTile.transform.position + Vector3.right * 10);
        _enemy = SpawnEnemy(currentTile.enemyPosition.position);
    }

    private Enemy SpawnEnemy(Vector3 position)
    {
        var enemy = Instantiate(enemyPrefab).GetComponent<Enemy>();
        enemy.transform.position = position;
        return enemy;
    }

    private Tile CreateNewTile(Vector3 position)
    {
        return Instantiate(tilePrefab, position, Quaternion.identity).GetComponent<Tile>();
    }

    [Button]
    public void Fight()
    {
        if (gameState != GameState.Resting)
            return;

        GoToAttacking();
    }

    private void ResetFight()
    {
        SetGameState(GameState.Resting);

        character.transform.position = currentTile.restingPosition.position;
        character.Health.HealFully();

        _enemy.Health.HealFully();
    }
}