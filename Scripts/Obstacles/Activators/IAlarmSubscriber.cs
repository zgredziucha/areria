using UnityEngine;
using System.Collections;

public interface IAlarmSubscriber {
	void TriggerBehaviour (bool isEnabled);
    void Reset();
}
