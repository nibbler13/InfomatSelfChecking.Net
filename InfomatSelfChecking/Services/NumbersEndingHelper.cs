using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
	public static class NumbersEndingHelper {
		/// <summary>
		/// Возвращает слова в падеже, зависимом от заданного числа 
		/// </summary>
		/// <param name="number">Число от которого зависит выбранное слово</param>
		/// <returns></returns>
		public static string GetDeclension(int number) {
			number %= 100;

			if (number >= 11 && number <= 19)
				return "минут";

			var i = number % 10;
			switch (i) {
				case 1:
					return "минуту";
				case 2:
				case 3:
				case 4:
					return "минуты";
				default:
					return "минут";
			}
		}
	}
}
