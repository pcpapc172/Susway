using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Minimal stubs for the Prime31 types used by the project.
// These are intentionally lightweight: they allow the project to compile
// when the original Prime31 plugin isn't present. They do NOT provide
// full networking functionality — they're placeholders so the rest of
// the codebase can build. Replace these with the real Prime31 library
// if you need the original behaviour.
namespace Prime31
{
    public enum HTTPVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class P31RestKit
    {
        // Fields mirrored from the original plugin that other code expects
        protected string _baseUrl;
        protected bool forceJsonResponse;
        protected MonoBehaviour surrogateMonobehaviour;

        public P31RestKit()
        {
            EnsureSurrogate();
        }

        private void EnsureSurrogate()
        {
            if (surrogateMonobehaviour != null)
                return;

            var go = GameObject.Find("P31RestKitSurrogate");
            if (go == null)
            {
                go = new GameObject("P31RestKitSurrogate");
                GameObject.DontDestroyOnLoad(go);
            }

            surrogateMonobehaviour = go.GetComponent<SurrogateBehaviour>();
            if (surrogateMonobehaviour == null)
                surrogateMonobehaviour = go.AddComponent<SurrogateBehaviour>();
        }

        // A tiny MonoBehaviour used only to host coroutines when code does
        // `surrogateMonobehaviour.StartCoroutine(...)`.
        private class SurrogateBehaviour : MonoBehaviour { }

        // Minimal send implementation: yields one frame and then invokes the
        // completion callback with a null result. This preserves call order
        // semantics without performing any network I/O.
        protected virtual IEnumerator send(string path, HTTPVerb httpVerb, Dictionary<string, object> parameters, Action<string, object> onComplete)
        {
            yield return null;
            onComplete?.Invoke(null, null);
        }

        // Convenience helpers that many Prime31-based wrappers expect.
        public void get(string path, Action<string, object> onComplete)
        {
            surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.GET, null, onComplete));
        }

        public void get(string path, Dictionary<string, object> parameters, Action<string, object> onComplete)
        {
            surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.GET, parameters, onComplete));
        }

        public void post(string path, Dictionary<string, object> parameters, Action<string, object> onComplete)
        {
            surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.POST, parameters, onComplete));
        }

        public void post(string path, Action<string, object> onComplete)
        {
            surrogateMonobehaviour.StartCoroutine(send(path, HTTPVerb.POST, null, onComplete));
        }
    }
}

