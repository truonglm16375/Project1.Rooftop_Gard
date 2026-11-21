using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileRemoverWindow : EditorWindow
{
    private Tilemap targetTilemap;
    private TileBase tileToRemove;

    // Tạo menu trên thanh công cụ của Unity
    [MenuItem("Tools/Tilemap/Xóa Tile Theo Loại")]
    public static void ShowWindow()
    {
        GetWindow<TileRemoverWindow>("Xóa Tile");
    }

    private void OnGUI()
    {
        GUILayout.Label("Công Cụ Xóa Tile Trong Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Ô kéo thả Tilemap và loại Tile cần xóa
        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap Mục Tiêu", targetTilemap, typeof(Tilemap), true);
        tileToRemove = (TileBase)EditorGUILayout.ObjectField("Loại Tile Cần Xóa", tileToRemove, typeof(TileBase), false);

        EditorGUILayout.Space();

        // Khóa nút bấm nếu chưa kéo đủ 2 dữ liệu trên
        GUI.enabled = (targetTilemap != null && tileToRemove != null);

        if (GUILayout.Button("Xóa Tất Cả Tile Này", GUILayout.Height(35)))
        {
            RemoveTiles();
        }

        GUI.enabled = true;
    }

    private void RemoveTiles()
    {
        // Ghi lại lịch sử để có thể bấm Ctrl + Z (Undo)
        Undo.RegisterCompleteObjectUndo(targetTilemap, "Remove Specific Tiles");

        // Xóa toàn bộ tile trùng khớp
        targetTilemap.SwapTile(tileToRemove, null);
        
        // Thu gọn lại viền giới hạn của Tilemap
        targetTilemap.CompressBounds();

        // Báo cho Unity biết Scene đã có thay đổi để lưu lại (Ctrl + S)
        EditorUtility.SetDirty(targetTilemap);

        Debug.Log($"Đã xóa sạch tile '{tileToRemove.name}' khỏi '{targetTilemap.name}'!");
    }
}
