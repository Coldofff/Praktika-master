using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Praktika
{
    class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            payments payment = (payments)value;
            var paymentsOfUser = Instances.db.payments.Where(p => p.FK_user_id == payment.FK_user_id).ToList();
            var top3 = paymentsOfUser.OrderByDescending(p => p.sum).ToList();
            var top3Bottom = paymentsOfUser.OrderBy(p => p.sum).ToList();
            top3Bottom = top3Bottom.GetRange(0, 3).ToList();
            top3 = top3.GetRange(0, 3).ToList();
            if(top3Bottom.LastOrDefault().sum >= payment.sum)
                return "Green";
            if (top3.LastOrDefault().sum <= payment.sum)
                return "Red";
            return "White";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
