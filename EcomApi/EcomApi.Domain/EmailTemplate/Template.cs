using EcomApi.Domain.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Domain.EmailTemplate
{
    public class Template
    {

        public static string emailTemplatwelcomee(string uname)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($@"
<html>
<head>
<style>
    body {{
        font-family: Arial, sans-serif;
        background-color: #f5f5f5;
        width: 100%;
    }}
    .container {{
        max-width: 600px;
        margin: 0 auto;
        padding: 20px;
        background-color: #fff;
        border-radius: 8px;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }}
    h1, h2 {{
        text-align: center;
        color: #333;
    }}
    p {{
        color: #9e9e9e;
        text-align: center;
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    }}
    .messg-topic {{
        color: #f3f3f3;
        text-align: center;
        font-size: 35px;
        font-weight: 600;
        position: absolute;
        bottom: 50%;
        left: 0;
        padding: 0 40px;
        line-height: 18px;
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    }}
    .messg {{
        color: #4d4d4d;
        text-align: center;
        padding: 0 40px;
        line-height: 22px;
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    }}
    .x {{
        line-height: 25px;
    }}
    .highlight {{
        color: #0092ac;
        font-weight: bold;
    }}
    .headding-sec {{
        background-color: #ffffff;
        color: #2c2c2c;
        font-weight: bold;
        padding: 15px 10px;
        font-size: 30px;
    }}
    .welcome {{
        margin-bottom: 80px;
    }}
    .welcome-image {{
        width: 100%;
        background-image: url('https://img.freepik.com/free-photo/female-friends-out-shopping-together_53876-25041.jpg?t=st=1716280947~exp=1716284547~hmac=fb834fd5a47b901d99b5151abe91bd908a470c7cc5bc4b1969c6949f00b72a54&w=900');
        background-size: cover;
        background-position: bottom;
        min-height: 300px;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        position: relative;
    }}
    .welcome-image div {{
        position: absolute;
        width: 100%;
        height: 300px;
        background-color: #004f633a;
    }}
    .welcome-image img {{
        width: 100%;
        object-fit: fill;
    }}
    .hr {{
        width: 100%;
        height: 1px;
        background-color: #e7e7e7 !important;
    }}
    .copyright {{
        font-size: 10px;
    }}
    .footer {{
        color: #818181;
    }}
    .name {{
        color: #0092ac;
    }}
</style>
</head>
<body>
<div class='container'>
    <div class='welcome-image'>
        <div></div>
    </div>
    <h1 class='headding-sec'>Welcome to <span class='name'>Shopping Bay!</span></h1>
    <div class='welcome'>
        <div></div>
        <p class='messg'>Dear <span class='highlight'>{uname}</span>,</p>
        <p class='messg'>Welcome to our platform! We are thrilled to have you join us.</p>
        <p class='messg x'>With our platform, you can enjoy a seamless shopping experience
            with a wide variety of products, competitive prices, fast checkout, and personalized recommendations. Benefit from exclusive deals,
            reliable customer support, and a hassle-free return policy for an enjoyable online shopping experience.</p>
        <p class='messg'></p>
        <p class='messg'>Until then, enjoy your shopping!</p>
    </div>
    <div class='hr'></div>
    <p class='footer'>If you need any assistance, feel free to contact our support team.</p>
    <p class='footer'>Thank you!</p>
    <p class='copyright'>The Shopping Bay Team @2024 All Right Reserved.</p>
</div>
</body>
</html>");

            var emailHtml = stringBuilder.ToString();
            return emailHtml;


        }

        public static string emailTemplate(Order order)
        {
            try
            {
                string useremail = order.User.Email;
                string name = order.User.first_Name;
                DateTime date = order.Order_Date;
                string payment_id = order.Payment_ID;

                var stringBuilder = new StringBuilder();
                var totalamount = (order.price) / 100;

                stringBuilder.AppendLine($@"
<html>
<head>
    <style>
        body {{
            font-family: 'Arial', sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .container {{
            width: 80%;
            margin: auto;
            background-color: #fff;
            padding: 20px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #F0F8FF;
            color: #13274F;
            padding: 20px 0;
            text-align: center;
            border-radius: 3px 3px 0 0;
        }}
        .header h1 {{
            margin: 0;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }}
        th, td {{
            padding: 10px;
            border: 0;
            text-align: left;
        }}
        th {{
            background-color: #F0F8FF;
            color: #13274F;
        }}
        .total {{
            text-align: right;
            margin-top: 20px;
        }}
        .total p {{
            font-size: 1.2em;
            font-weight: bold;
            margin: 5px 0;
        }}
        .status {{
            font-weight: bold;
            color: #28a745;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Invoice</h1>
        </div>
        <p>Dear {name},</p>
        <p>Your order is complete. Your order details are as follows:</p>
<p>Payment ID: {payment_id}</p>
<p>Order ID: {order.Order_Id}</p>
<p>Order Date: {date}</p>
        <table>
            <tr>
                <th>Product Name</th>
                <th>Quantity</th>
                <th>Unit Price</th>
                <th>Total Price</th>
            </tr>");

                // Assuming 'emailOrder.OrderProducts' is a list of order products
                foreach (var orderProduct in order.OrderProducts)
                {
                    {
                        var totalProductPrice = orderProduct.Order_Qty * orderProduct.Unite_Price;

                        string formattedTotalProductPrice = totalProductPrice.ToString("N0");
                        stringBuilder.AppendLine($@"
            <tr>
                <td>{orderProduct.product.Name}</td>
                <td>{orderProduct.Order_Qty}</td>
                <td>${orderProduct.Unite_Price.ToString("N0")}</td>
                <td>${formattedTotalProductPrice}</td>
            </tr>");
                    }
                }

                stringBuilder.AppendLine($@"
        </table>
        <div class='total'>
            <p>Total: ${totalamount.ToString("N0")}</p>
            <p class='status'>Payment Status: {order.Order_Status}</p>
        </div>
    </div>
</body>
</html>");


                string message = stringBuilder.ToString();
                return message;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating email template for order ID: {OrderId}", order.Order_Id);
                throw new Exception("There was an error generating the email template. Please try again later.");
            }
        }
    }
}
