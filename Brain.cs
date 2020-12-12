using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
public class Brain : MonoBehaviour
{
    Queue<Message> messages = new Queue<Message>();
    public void DoUpdate(Message latest)
    {
        messages.Enqueue(latest);
    }
}
}