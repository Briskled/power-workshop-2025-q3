using Cysharp.Threading.Tasks;
using DG.Tweening;
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
    [Space] [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private CinemachineCamera shopCamera;

    [SerializeField] private Tile currentTile;
    private Enemy _enemy;
    private Tile _previousTile;
    private Tile _nextTile;
    private int _enemyLevel = 1;

    public void SetGameState(GameState newGameState)
    {
        var stateBefore = gameState;
        gameState = newGameState;
        GlobalEvents.onGameStateChanged?.Invoke(new GameStateChangedEvent
        {
            stateBefore = stateBefore,
            stateNow = gameState
        });
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
        await character.WalkTo(currentTile.restingPosition.position.x);
        GoToResting();
    }

    private void GoToResting()
    {
        SetGameState(GameState.Resting);
        FocusEnemy(_enemy);
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
            FocusCharacterOnly();
            character.Health.HealFully();
            _enemyLevel++;
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
        enemy.Level = _enemyLevel;
        return enemy;
    }

    private Tile CreateNewTile(Vector3 position)
    {
        return Instantiate(tilePrefab, position, Quaternion.identity).GetComponent<Tile>();
    }

    private void FocusEnemy(Enemy enemy)
    {
        var newTarget = new CinemachineTargetGroup.Target
        {
            Object = enemy.transform,
            Radius = 1,
            Weight = 0
        };
        var index = targetGroup.Targets.Count;
        targetGroup.Targets.Add(newTarget);

        DOTween.To(
            () => targetGroup.Targets[index].Weight,
            x => targetGroup.Targets[index].Weight = x,
            1,
            1);
    }

    private void FocusCharacterOnly()
    {
        targetGroup.Targets.RemoveAll(it => it.Object.TryGetComponent<Enemy>(out _));
    }

    [Button]
    public void Fight()
    {
        if (gameState != GameState.Resting)
            return;

        GoToAttacking();
    }

    [Button]
    public void GoToShopping()
    {
        if (gameState != GameState.Resting)
            return;

        SetGameState(GameState.Shopping);
        shopCamera.gameObject.SetActive(true);
    }

    public void LeaveShopping()
    {
        GoToResting();
        shopCamera.gameObject.SetActive(false);
    }

    private void ResetFight()
    {
        SetGameState(GameState.Resting);

        character.transform.position = currentTile.restingPosition.position;
        character.Health.HealFully();

        _enemy.Health.HealFully();
    }
}