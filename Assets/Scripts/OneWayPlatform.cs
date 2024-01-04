using UnityEngine;
using UnityEngine.Tilemaps;

public class OneWayPlatform : MonoBehaviour
{
    // Referenz auf die Tilemap
    public Tilemap tilemap;

    // Legt fest, von welchen Seiten aus der Boden durchspringbar ist
    public bool canPassFromLeft = true;
    public bool canPassFromRight = true;
    public bool canPassFromBottom = true;

    // Wird aufgerufen, wenn ein Objekt den Trigger betritt
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 collisionPosition = collision.transform.position;

        // Überprüft, ob das Objekt von der richtigen Seite kommt
        if ((canPassFromLeft && collisionPosition.x < transform.position.x) ||
            (canPassFromRight && collisionPosition.x > transform.position.x) ||
            (canPassFromBottom && collisionPosition.y < transform.position.y))
        {
            // Überprüfe Kollision mit der Tilemap
            Vector3Int cellPosition = tilemap.WorldToCell(collisionPosition);
            TileBase tile = tilemap.GetTile(cellPosition);

            if (tile != null)
            {
                // Setze die Kollision zurück
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), true);
            }
        }
    }

    // Wird aufgerufen, wenn ein Objekt den Trigger verlässt
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Setzt die Kollision wieder zurück
        Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), false);
    }
}