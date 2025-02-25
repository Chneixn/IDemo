using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataService
{
    void Save(ISaveData data, bool overwrite = true);
    ISaveData Load(string name);
    void Delete(string name);
    void DeleteAll();
    IEnumerable<string> ListSaves();

}
