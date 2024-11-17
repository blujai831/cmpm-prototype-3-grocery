using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering; // to access SortingGroup
using System;
using TMPro;

// This class manages the spawning and scanning of groceries in a game
public class SpawnManage : MonoBehaviour
{
    [SerializeField] private Button scan_button; // Button to trigger scanning
    [SerializeField] private GameObject[] groceryPrefabs; // Array of grocery prefabs to spawn
    [SerializeField] private TMP_Text score_text; // Text to display the current score
    private int score; // The player's current score
    [SerializeField] private int num_groceries_to_spawn; // Number of groceries to spawn each round
    [SerializeField] HashSet<GameObject> barcodes_in_scanner = new HashSet<GameObject>(); // A set to store scanned barcodes in the scanner

    // Initializes variables and spawns the initial grocery
    void Start()
    {
        score = 0;
        num_groceries_to_spawn = 1;
        Spawn_Groceries();

        // Adds a listener to the scan button to call the Scan_Groceries method when clicked
        scan_button.onClick.AddListener(Scan_Groceries);
    }

    // Detects if a barcode stays within the scanner's trigger area
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Check if the collided object has a "Barcode" tag
        if (collision.gameObject.CompareTag("Barcode"))
        {
            // Add to scanner if oriented correctly and not already in scanner
            if (CorrectlyOriented(collision.gameObject) && !barcodes_in_scanner.Contains(collision.gameObject))
            {
                barcodes_in_scanner.Add(collision.gameObject);
            }
            // Remove from scanner if oriented incorrectly and already in scanner
            else if (!CorrectlyOriented(collision.gameObject) && barcodes_in_scanner.Contains(collision.gameObject))
            {
                barcodes_in_scanner.Remove(collision.gameObject);
            }
        }
    }

    // Removes barcodes from the scanner if they exit the trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Barcode") && barcodes_in_scanner.Contains(collision.gameObject))
        {
            barcodes_in_scanner.Remove(collision.gameObject);
        }
    }

    // Called when the scan button is pressed to tally points and remove groceries
    void Scan_Groceries()
    {
        // If all spawned groceries are in the scanner, proceed to scan and score them
        if (barcodes_in_scanner.Count == num_groceries_to_spawn)
        {
            // List to store barcodes for destruction after scanning
            List<GameObject> barcodesToDestroy = new List<GameObject>();

            // Iterate through each barcode in the scanner and add to the score
            foreach (var barcode in barcodes_in_scanner)
            {
                score++;
                barcodesToDestroy.Add(barcode); // Add to the list for later removal
            }

            // Destroy the barcodes and clear the HashSet
            foreach (var barcode in barcodesToDestroy)
            {
                Destroy(barcode.transform.parent.gameObject); // Destroys the parent (grocery item)
                barcodes_in_scanner.Remove(barcode); // Safe to modify here
            }

            // Spawn new groceries after scoring
            Spawn_Groceries();
        }

        // Update the score UI to reflect the new score
        score_text.text = $"Score: {score}";
    }

    // Spawns a random number of groceries in the game world
    void Spawn_Groceries()
    {
        // Determines a random number of groceries to spawn
        number_to_spawn();

        for (int i = 0; i < num_groceries_to_spawn; i++)
        {
            // Select a random grocery prefab
            int grocery_index = UnityEngine.Random.Range(0, groceryPrefabs.Length);
            // Instantiate the grocery at a fixed position with a default rotation
            float zOffset = -0.1f * i; // Sets a small offset to each consecutive item spawned
            Vector2 spawnPosition = new Vector3(-7.5f, 3.5f, zOffset);
            spawnPosition += new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
            GameObject grocery = Instantiate(groceryPrefabs[grocery_index], spawnPosition, Quaternion.identity);

            // Apply Sorting Group settings
            SortingGroup sortingGroup = grocery.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = grocery.AddComponent<SortingGroup>();
            }

            sortingGroup.sortingLayerName = "Grocery"; // Ensure the grocery layer is set correctly
            sortingGroup.sortingOrder = i; // Increment order for stacking effect


            // Possible solution to items clumping together:
            // setting isKinematic to true makes the items not interact with each other physically. But it also prevents the user from clicking and dragging
            // items. Maybe we set them as kinematic and then un-kinematic them when the player wants to pick them up? Maybe we assign ordering to the items
            // that are spawned and let the top item be un-kinematic and the rest kinematic, until the top item is picked up?
            // for TLDR: message isaac for implementation ideas

            //Rigidbody2D rb = grocery.GetComponent<Rigidbody2D>();
            //rb.isKinematic = true;
                // I think this is probably the way to go, but should be done in Grocery.cs, since that's where we detect mouse clicks. - Jaime

            // Alternative possible solution:
            // Shapecast and move out of overlap

            /*Collider2D collider = grocery.GetComponent<Collider2D>();
            var filter = new ContactFilter2D();
            filter.NoFilter();
            var hits = new List<RaycastHit2D>();
            for (;;) {
                collider.Cast(Vector3.right, filter, hits);
                bool any = false;
                foreach (var hit in hits) {
                    if (!hit.collider.isTrigger) {
                        any = true;
                        break;
                    }
                }
                hits.Clear();
                if (any) {
                    grocery.transform.position += Vector3.right;
                    Physics2D.SyncTransforms();
                } else {
                    break;
                }
            }*/
        }
    }

    // Checks if the barcode on a grocery item is correctly oriented for scanning
    public bool CorrectlyOriented(GameObject barcode)
    {
        // Returns true if the barcode's rotation angle is close to 0 (horizontal) or 180 degrees
        const double target = 0.1736;
        return Math.Abs(Math.Sin(barcode.transform.rotation.eulerAngles.z * Math.PI / 180.0)) <= target; // margin of error: 10 degrees
        // Equation to calculate margin of error (if you want to change it): target = sin(margin you want in degrees)
    }

    // Determines a random number of groceries to spawn for the next round
    void number_to_spawn()
    {
        num_groceries_to_spawn = (int)UnityEngine.Random.Range(2, 4);
    }
}
