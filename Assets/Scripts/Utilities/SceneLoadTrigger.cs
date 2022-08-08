using UnityEngine;

public class SceneLoadTrigger : Interactable
{
    public SceneInfo sceneToLoad;
    public string message;

    public override bool OnInteract()
    {
        DialogManager.instance.ShowQuestion(message, () =>
        {
            EnterScene();
        }, () => {
            Debug.Log("That's a no on the sale!");
        });

        return base.OnInteract();
    }

    private void EnterScene()
    {
        GameManager.instance.LoadNewScene(sceneToLoad);
    }
}
