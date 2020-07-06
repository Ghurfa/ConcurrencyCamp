using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonogameConcurrency.Synchronization
{
    public class MonoGameSynchronizationContext : SynchronizationContext
    {
        public struct PostPair
        {
            public SendOrPostCallback Callback;
            public object State;
            public PostPair(SendOrPostCallback d, object state)
            {
                Callback = d;
                State = state;
            }
        }
        object lockObj = new object();
        List<PostPair> postObjs = new List<PostPair>();
        List<PostPair> postObjs2 = new List<PostPair>();

        /// <summary>
        /// Takes post message from any thread and puts it into postObjs, to be run on main thread by ExecuteContinuations
        /// </summary>
        public override void Post(SendOrPostCallback d, object state)   
        {
            lock(lockObj)
            {
                postObjs.Add(new PostPair(d, state));
            }
            //base.Post(d, state);
        }

        /// <summary>
        /// Blocking version of Post
        /// </summary>
        public override void Send(SendOrPostCallback d, object state)
        {
            throw new InvalidOperationException("Cannot send");
        }

        /// <summary>
        /// Function called by main thread to run all post objects
        /// </summary>
        public void ExecuteContinuations()
        {
            //Swap references - minimize time with lock
            lock(lockObj)
            {
                List<PostPair> temp = postObjs;
                postObjs = postObjs2;
                postObjs2 = temp;
                postObjs.Clear();
            }
            foreach(PostPair pair in postObjs2)
            {
                pair.Callback(pair.State);
            }
        }
    }
}
