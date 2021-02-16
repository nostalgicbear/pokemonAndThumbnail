using GraphQlClient.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;

public class Pokemon : MonoBehaviour
{
    [SerializeField]
    private GameObject pokemonAttackDetailsPrefab;
    
    [SerializeField]
    private Transform grid;

    [SerializeField]
    private TextMeshProUGUI selectedPokemonText;

    [SerializeField]
    private GraphApi pokemonGraph;

    private string lastPokemonQueried = "";

    /// <summary>
    /// Gets the attack details of a pokemon via the PokemonAttackDetails query
    /// </summary>
    /// <param name="pokemonToQuery"></param>
    public async void GetPokemonAttackDetails(string pokemonToQuery)
    {
        if(lastPokemonQueried.ToLower() == pokemonToQuery.ToLower())
        {
            return;
        }

        GraphApi.Query query = pokemonGraph.GetQueryByName("PokemonAttackDetails", GraphApi.Query.Type.Query);
        query.SetArgs(new { name = pokemonToQuery });
        UnityWebRequest request = await pokemonGraph.Post(query);

        ParseResponse(request.downloadHandler.text); //Parse the response from the request
    }

    private void ParseResponse(string requestResponse)
    {
        ClearAttackDetailsEntries(); //Clear entries as we dont want an additive list of attacks

        JSONNode data = JSON.Parse(requestResponse);

        selectedPokemonText.text = data["data"]["pokemon"]["name"].Value;
        lastPokemonQueried = selectedPokemonText.text;

        JSONNode fastAttacks = data["data"]["pokemon"]["attacks"]["fast"];
        JSONNode specialAttacks = data["data"]["pokemon"]["attacks"]["special"];

        for (int i = 0; i < fastAttacks.Count; i++) //Here I separate out the attacks (fast or special)
        {
            CreateGridEntry(fastAttacks[i], true);
        }

        for(int i = 0; i < specialAttacks.Count; i++)
        {
            CreateGridEntry(specialAttacks[i], false);
        }
    }

    /// <summary>
    /// Deletes existing "Attack stat" cards that are already present in the ui
    /// </summary>
    private void ClearAttackDetailsEntries()
    {
        Debug.Log("Clearing data");
        if (grid.childCount > 0)
        {
            for(int i =0; i < grid.childCount; i++)
            {
                Destroy(grid.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// Creates an entry on the UI
    /// </summary>
    /// <param name="node">JSON data related to a specific attack</param>
    /// <param name="isFastAttackEntry">Whether or not the attack is a fast attack or a special attack</param>
    private void CreateGridEntry(JSONNode node, bool isFastAttackEntry)
    {
        GameObject gridEntry = Instantiate(pokemonAttackDetailsPrefab, grid.transform);

        AttackDetailsGridEntry attackDetailsEntry = gridEntry.GetComponent<AttackDetailsGridEntry>();
        attackDetailsEntry.SetDetails(node["name"], node["type"], node["damage"], isFastAttackEntry);
    }
}
