using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class StackedTileBehaviour : MonoBehaviour
{
    private const string GeneratedTileNamePrefix = "__StackedTile_";
    private const string MainTilesetSpritePrefix = "MainTileset_";
    private static readonly string[] LegacyTilePrefixes = {"__ColoredTile_", "__StaticTile_"};
#if UNITY_EDITOR
    private const string MainTilesetAssetPath = "Assets/Images/MainTileset.png";
#endif

    [SerializeField]
    [Min(1)]
    private int _tileAmount = 1;

    [SerializeField]
    private string _tileValue = "A";

    [SerializeField]
    [Min(0f)]
    private float _distance;

    [SerializeField]
    private Vector3 _tileRotation = new Vector3(90f, 0f, 0f);

    private readonly List<SpriteRenderer> _tileRenderers = new List<SpriteRenderer>();

    protected IReadOnlyList<SpriteRenderer> TileRenderers => _tileRenderers;
    protected int TileCount => _tileRenderers.Count + 1;
    protected SpriteRenderer RootRenderer => GetComponent<SpriteRenderer>();
    public int TileAmount => _tileAmount;
    public string TileValue => _tileValue;

    protected virtual void Awake()
    {
        RebuildTiles();
    }

    protected virtual void OnDestroy()
    {
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (Application.isPlaying)
        {
            return;
        }

        ClearGeneratedTiles();
        ConfigureRootTile();
        _tileRenderers.Clear();
        RefreshColors();
    }
#endif

    public void SetTileValue(string tileValue)
    {
        _tileValue = tileValue;

        var sprite = ResolveConfiguredSprite();
        if (sprite == null)
        {
            return;
        }

        RootRenderer.sprite = sprite;

        for (var i = 0; i < _tileRenderers.Count; i++)
        {
            _tileRenderers[i].sprite = sprite;
        }
    }

    public void SetTileAmount(int tileAmount)
    {
        if (tileAmount <= 0)
        {
            OnTileAmountDepleted();
            Destroy(gameObject);
            return;
        }

        _tileAmount = tileAmount;
        if (RootRenderer == null)
        {
            return;
        }

        RebuildTiles();
    }

    protected void RefreshColors()
    {
        RootRenderer.color = GetTileColor(0);

        for (var i = 0; i < _tileRenderers.Count; i++)
        {
            _tileRenderers[i].color = GetTileColor(i + 1);
        }
    }

    protected abstract Color GetTileColor(int tileIndex);

    protected virtual void OnTileAmountDepleted()
    {
    }

    private void RebuildTiles()
    {
        ClearGeneratedTiles();
        ConfigureRootTile();

        _tileRenderers.Clear();

        for (var i = 1; i < _tileAmount; i++)
        {
            var child = new GameObject($"{GeneratedTileNamePrefix}{i}");
            child.transform.SetParent(transform, false);

            var childRenderer = child.AddComponent<SpriteRenderer>();
            ConfigureTileRenderer(childRenderer, i);
            _tileRenderers.Add(childRenderer);
        }

        RefreshColors();
    }

    private void ConfigureRootTile()
    {
        _tileAmount = Mathf.Max(1, _tileAmount);
        _distance = Mathf.Max(0f, _distance);
        transform.localRotation = Quaternion.Euler(_tileRotation);

        var sprite = ResolveConfiguredSprite();
        if (sprite != null)
        {
            RootRenderer.sprite = sprite;
        }

    }

    private void ConfigureTileRenderer(SpriteRenderer render, int tileIndex)
    {
        render.sprite = RootRenderer.sprite;
        render.sharedMaterial = RootRenderer.sharedMaterial;
        render.sortingLayerID = RootRenderer.sortingLayerID;
        render.sortingOrder = RootRenderer.sortingOrder;
        render.maskInteraction = RootRenderer.maskInteraction;
        render.drawMode = RootRenderer.drawMode;
        render.size = RootRenderer.size;
        render.flipX = RootRenderer.flipX;
        render.flipY = RootRenderer.flipY;
        render.enabled = RootRenderer.enabled;

        var tileTransform = render.transform;
        tileTransform.localPosition = Vector3.back * (_distance * tileIndex);
        tileTransform.localRotation = Quaternion.identity;
        tileTransform.localScale = Vector3.one;
    }

    private void ClearGeneratedTiles()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);

            if (!IsGeneratedTile(child.name))
            {
                continue;
            }

            if (Application.isPlaying)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
                continue;
            }

            DestroyImmediate(child.gameObject);
        }
    }

    private static bool IsGeneratedTile(string childName)
    {
        if (childName.StartsWith(GeneratedTileNamePrefix, StringComparison.Ordinal))
        {
            return true;
        }

        for (var i = 0; i < LegacyTilePrefixes.Length; i++)
        {
            if (childName.StartsWith(LegacyTilePrefixes[i], StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private Sprite ResolveConfiguredSprite()
    {
        var spriteName = Helper.GetMainTilesetSpriteName(_tileValue);
#if UNITY_EDITOR
        var configuredSprite = LoadMainTilesetSprite(spriteName);

        if (configuredSprite != null)
        {
            return configuredSprite;
        }
#endif

        var runtimeSprite = LoadRuntimeMainTilesetSprite(spriteName);
        if (runtimeSprite != null)
        {
            return runtimeSprite;
        }

        return RootRenderer.sprite;
    }

    private Sprite LoadRuntimeMainTilesetSprite(string spriteName)
    {
        var templateSprite = RootRenderer != null ? RootRenderer.sprite : null;
        if (templateSprite == null || templateSprite.texture == null || !IsMainTilesetSprite(templateSprite))
        {
            return null;
        }

        var spriteIndex = Helper.GetMainTilesetIndex(_tileValue);
        var normalizedPivot = GetNormalizedPivot(templateSprite);

        // Rebuild the sliced sprite from the tileset texture so value changes also work in player builds.
        var runtimeSprite = Sprite.Create(
            templateSprite.texture,
            Helper.GetMainTilesetSpriteRect(spriteIndex),
            normalizedPivot,
            templateSprite.pixelsPerUnit,
            1,
            SpriteMeshType.FullRect,
            templateSprite.border);

        runtimeSprite.name = spriteName;
        return runtimeSprite;
    }

    private static bool IsMainTilesetSprite(Sprite sprite)
    {
        return sprite.name.StartsWith(MainTilesetSpritePrefix, StringComparison.Ordinal);
    }

    private static Vector2 GetNormalizedPivot(Sprite sprite)
    {
        var spriteSize = sprite.rect.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
        {
            return new Vector2(0.5f, 0.5f);
        }

        return new Vector2(sprite.pivot.x / spriteSize.x, sprite.pivot.y / spriteSize.y);
    }

#if UNITY_EDITOR
    private static Sprite LoadMainTilesetSprite(string spriteName)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath(MainTilesetAssetPath);

        foreach (var asset in assets)
        {
            if (asset is Sprite sprite && sprite.name == spriteName)
            {
                return sprite;
            }
        }

        return null;
    }
#endif
}
