using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    private InputSystem_Actions inputs;

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 direction;
    private bool enSuelo = true;
    private int contactosSuelo = 0;

    void Start()
    {
        inputs = new InputSystem_Actions();
        inputs.Enable();
        inputs.Player.Jump.started += Jump;
    }

    void OnDestroy()
    {
        if (inputs != null)
        {
            inputs.Player.Jump.started -= Jump;
            inputs.Disable();
        }
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        // Si este script está apagado, no hace nada (evita que salte el otro personaje)
        if (!enabled) return;

        // Si no está tocando el suelo, no puede saltar
        if (!enSuelo) return;

        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0);
        rigid.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        enSuelo = false;
    }

    private void FixedUpdate()
    {
        rigid.linearVelocity = new Vector2(direction.x, rigid.linearVelocity.y);
    }

    private bool EsSuperficieValida(GameObject obj)
    {
        return obj.CompareTag("Suelo") ||
               obj.CompareTag("Player") ||
               obj.CompareTag("Player2") ||
               obj.CompareTag("Player3") ||
               obj.CompareTag("Player4") ||
               obj.CompareTag("Player5");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!EsSuperficieValida(collision.gameObject)) return;

        contactosSuelo++;
        enSuelo = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!EsSuperficieValida(collision.gameObject)) return;

        contactosSuelo = Mathf.Max(0, contactosSuelo - 1);
        if (contactosSuelo == 0)
        {
            enSuelo = false;
        }
    }

    void Update()
    {
        // Si el script está apagado, la dirección es cero para que no se mueva
        if (!enabled)
        {
            direction = Vector2.zero;
            return;
        }

        direction = inputs.Player.Move.ReadValue<Vector2>() * moveSpeed;
    }
}
