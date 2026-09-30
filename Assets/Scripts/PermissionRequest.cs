using UnityEngine;
using UnityEngine.Android;

public class PermissionRequest : MonoBehaviour
{
    private string scenePermission = "com.oculus.permission.USE_SCENE"; //Gets the permission from the Android Manifest

    void Start()
    {
        RequestPermissionIfNeeded(scenePermission);

    }

    void RequestPermissionIfNeeded(string permission)
    {
        if (!Permission.HasUserAuthorizedPermission(permission)) //Checks if permission has been given
        {
            var callbacks = new PermissionCallbacks(); //Creates the variable to store the users choice
            Permission.RequestUserPermission(permission, callbacks);  //Asks the permission
        }
    }
}