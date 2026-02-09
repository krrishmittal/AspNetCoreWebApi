using System.Security.Claims;
using JwtAuth.Service;

namespace JwtAuth.Helpers
{
    public static class ClaimsHelper
    {
        public static int GetCurrentUserId(ClaimsPrincipal user, EncryptionService encryptionService)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? user.FindFirst("sub");
            
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            // Decrypt the encrypted user ID
            var decryptedUserId = encryptionService.Decrypt(userIdClaim.Value);
            
            if (!int.TryParse(decryptedUserId, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            
            return userId;
        }

        public static string GetCurrentUserRole(ClaimsPrincipal user, EncryptionService encryptionService)
        {
            var roleClaim = user.FindFirst(ClaimTypes.Role);
            
            if (roleClaim == null)
            {
                throw new UnauthorizedAccessException("Role not found in token");
            }

            // Decrypt the encrypted role
            return encryptionService.Decrypt(roleClaim.Value);
        }

        public static string GetCurrentUsername(ClaimsPrincipal user, EncryptionService encryptionService)
        {
            var nameClaim = user.FindFirst(ClaimTypes.Name) 
                         ?? user.FindFirst("name");
            
            if (nameClaim == null)
            {
                throw new UnauthorizedAccessException("Username not found in token");
            }

            // Decrypt the encrypted username
            return encryptionService.Decrypt(nameClaim.Value);
        }
    }
}