using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SpawnManage : MonoBehaviour
{
    [SerializeField] private Button scan_button;
    [SerializeField] private GameObject[] groceryPrefabs;
    [SerializeField] private TMP_Text score_text;
    private int score;

    [SerializeField] private int num_groceries_to_spawn;
    [SerializeField] HashSet<GameObject> barcodes_in_scanner = new HashSet<GameObject>();

    void Start()
    {
        score = 0;
        num_groceries_to_spawn = 1;
        Spawn_Groceries();

        scan_button.onClick.AddListener(Scan_Groceries);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Barcode"))
        {
            if (CorrectlyOriented(collision.gameObject) && !barcodes_in_scanner.Contains(collision.gameObject))
            {
                barcodes_in_scanner.Add(collision.gameObject);
            }
            else if(!CorrectlyOriented(collision.gameObject) && barcodes_in_scanner.Contains(collision.gameObject))
            {
                barcodes_in_scanner.Remove(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Barcode") && barcodes_in_scanner.Contains(collision.gameObject))
        {
            barcodes_in_scanner.Remove(collision.gameObject);
        }
    }

    void Scan_Groceries()
    {
        if (barcodes_in_scanner.Count == num_groceries_to_spawn)
        {
            // Create a list to store barcodes to destroy
            List<GameObject> barcodesToDestroy = new List<GameObject>();

            foreach (var barcode in barcodes_in_scanner)
            {
                score++;
                barcodesToDestroy.Add(barcode); // Add to the list for later removal
            }

            // Destroy the barcodes and clear the HashSet
            foreach (var barcode in barcodesToDestroy)
            {
                Destroy(barcode.transform.parent.gameObject);
                barcodes_in_scanner.Remove(barcode); // Safe to modify here
            }

            Spawn_Groceries();
        }

        // Update the score UI
        score_text.text = $"Score: {score}";
    }


    void Spawn_Groceries()
    {
        number_to_spawn();

        for (int i = 0; i < num_groceries_to_spawn; i++)
        {
            int grocery_index = UnityEngine.Random.Range(0, groceryPrefabs.Length);
            GameObject grocery = Instantiate(groceryPrefabs[grocery_index], new Vector2(-7.5f, 3.5f), Quaternion.identity);

            float random_rotation = UnityEngine.Random.Range(0f, 360f);
            grocery.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, random_rotation);
        }
    }


    public bool CorrectlyOriented(GameObject barcode)
    {
        return Math.Abs(Math.Sin(barcode.transform.rotation.eulerAngles.z * Math.PI / 180.0)) <= 0.125;
    }

    void number_to_spawn()
    {
        num_groceries_to_spawn = (int)UnityEngine.Random.Range(1, 3);
    }
}
