// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SuE0dIJFqmt4j6AqerA/yDqmUlLVQevegiVKt9OSHGretZDhqyLi5eNR0vHj3tXa+VWbVSTe0tLS1tPQRlFT3sY7Yvb8A1ZJg0cKvaS5qxLR6m2m3sX3De3dCr3nd/dzsbOmwIqnnPXb57Wg2XAn9AuuT9IFxBaHhK4PpoYMcpigshoNLiQVYzemA1dPRilGIijMQ58ujfaJijkpkXSPi5JPMAl5bGW5N6nE1xCCpVppMB3XWm42BM9yA7Fzhhp7bZIM4IpLAXQtomfl+ElUyZyeo26laRJJu7Z3hwV/DEsWja+13OWB++QYMUE82Dpm5Uzk1vOCWVXyjUqyk9R3vEtDPJdR0tzT41HS2dFR0tLTYgp9MMgBUDEXPj2Rw/lbwtHQ0tPS");
        private static int[] order = new int[] { 3,2,3,7,11,9,8,10,9,9,11,12,13,13,14 };
        private static int key = 211;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
