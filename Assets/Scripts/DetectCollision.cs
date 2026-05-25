using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector3 _feetPos;
    private Vector3 _realFeetPos => transform.position + _feetPos;
    private const float AROUND_VALUE = 0.25f;
    [SerializeField] private float _checkDistance = 0.2f;
    public bool IsGrounded => IsOnGround();
    public bool IsGroundedAround => IsGroundAround();
    public LayerMask groundLayer => _groundLayer;

    private bool IsOnGround()
    {
        bool isGrounded = Physics.Raycast(_realFeetPos, Vector3.down, _checkDistance, _groundLayer);
        if (!isGrounded)
            isGrounded = IsGroundAround();
        return isGrounded;
    }
    private bool IsGroundAround()
    {
        var realFPos = _realFeetPos;
        realFPos.y -= _checkDistance;

        var directions = new[] { transform.forward, -transform.forward, transform.right, -transform.right };
        foreach (var direction in directions)
        {
            var aroundPos = realFPos + direction * AROUND_VALUE;
            var dir = (aroundPos - realFPos).normalized;
            var dist = Vector3.Distance(aroundPos, realFPos);
            if (Physics.Raycast(realFPos, dir, dist, _groundLayer))
            {
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(transform.position + _feetPos, _checkDistance);
    }
#endif
}
