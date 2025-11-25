This program is build using .NET8.0 and is WPF based.
We do use separate windows for each screen for simplicity and ease of development, so the program will be closing windows and opening new ones often that is normal.
(NOTE: The program needs to be in a folder where it can create new files. The files it will create are "users.json" "menuitems.json" "orders.json" and they are required for the functionality of the program.)
(NOTE: The program GUI is built specifically for the preset window size. For the best experience don't resize any of the windows inside the program otherwise the GUI might look weird.)

Instructions:

1. To get started you can create a new account through the sign up window it does function if you input the valid information.
Requirements for registration:
For the name the program will accept anything as long as the name is longer then 1 character. [Example: John Doe]
The phone number the requirement is 10 digits with or without the dashes it is not case sensitive. [Example: 1234567980, 123-456-7890, (123) 456-7890] These are all interpreted by the program as just the digits and will all work.
For the email the program will accept anything as long as there is a "@" symbol followed by a "." symbol. [Example: johndoe@example.com] The domain or ending does not matter it just needs to be a valid type of email format.
The password requirement is a single number 0-9, a capital letter A-Z, and it needs to be longer then 8 characters. [Example: Test12345, A12345678, ABCDEFGH1] This password is case sensitive and will be matched.

2. You can now sign in to the program using the newly registered account information. Again phone number is not case sensitive the password is. If you forgot your password you can reset it by entering all of the other information associated with the account. (If you forgot that too you can cheat and open the users.json file and directly edit it.)

3. Once you log in the program will directly open to the main menu. At the top in the header you have a few buttons one being the company information button, home page button (you are here), account information button, and cart button. You can add as many items as you want to the cart, however they will be removed from the cart if you leave the main menu screen. The reason for this is main menu closes as soon as another window opens, and it clears the cart. Despite this if you proceed to checkout through the cart it will pass your order to the next page so then it will be saved in the final order.

4. Build your pizza. The minimal requirements to build a custom pizza are a size, crust, and sauce option to be selected. The rest is optional, but recommended. (NOTE: Cheese is in the Vegetables section as per the instruction powerpoint.) When finished you can add the item to the cart and it should appear. If you want to remove an item from the cart you can do so by setting the quantity to 0, clicking the trash can button on the item, or clearing the entire cart.

5. Checkout. You need to select how you would like to receive your pizza and how you will pay for it. Pickup and Cash don't require any additional user input. Delivery requires a input address and card requires a input card information. In the address, Street accepts both numbers and letters, City and State accept only letters, and ZIP accepts only numbers. The minimum requirement is 1 character per field, so anything will be accepted as "valid". As for the card there is a minimum on all of the fields. The card number requires a minimum of 16 digits not case sensitive. Cardholder name requires at least 1 letter "space" and another letter as in first name last name format, expiry date requires a minimum 4 numbers again not case sensitive, security code requires a minimum of 3 numbers. [Examples: | Street: 123 First Street | City: Atlanta | State : Georgia | ZIP: 30001 | Cardholder Name: John Doe | Card Number: 1234-5768-9012-3456 | Expiry Date: 01/26 | Security Code: 123]

6. Review Page. Here no user input is required other then reviewing the information and changing it if needed. Then you can place your order.

7. About Us Page. Here there is just information about the store.

8. Account Information Page. Here you can view past receipts, and change account information. Same requirements apply as the registration/login pages.