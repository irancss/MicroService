// MongoDB initialization script
db = db.getSiblingDB('NotificationService');

// Create collections
db.createCollection('notification_templates');
db.createCollection('notification_logs');

// Create indexes for better performance
db.notification_templates.createIndex({ "Name": 1 }, { unique: true });
db.notification_templates.createIndex({ "Type": 1 });
db.notification_templates.createIndex({ "IsActive": 1 });
db.notification_templates.createIndex({ "Language": 1 });

db.notification_logs.createIndex({ "UserId": 1 });
db.notification_logs.createIndex({ "Type": 1 });
db.notification_logs.createIndex({ "Status": 1 });
db.notification_logs.createIndex({ "SentAt": -1 });
db.notification_logs.createIndex({ "TemplateId": 1 });

// Insert sample templates
db.notification_templates.insertMany([
    {
        "_id": ObjectId(),
        "Name": "user_registered_notification",
        "Type": 1, // Email
        "Subject": "Welcome to {{ company_name }}!",
        "Body": "<h1>Welcome {{ firstName }} {{ lastName }}!</h1><p>Thank you for registering with us on {{ registeredAt | date.to_string '%Y-%m-%d' }}.</p><p>We're excited to have you as part of our community!</p><p>Best regards,<br>The {{ company_name }} Team</p>",
        "Language": "en",
        "Parameters": {
            "firstName": "User's first name",
            "lastName": "User's last name",
            "registeredAt": "Registration date",
            "company_name": "Company name"
        },
        "IsActive": true,
        "CreatedAt": new Date(),
        "UpdatedAt": new Date(),
        "CreatedBy": "System",
        "UpdatedBy": "System"
    },
    {
        "_id": ObjectId(),
        "Name": "order_placed_notification",
        "Type": 1, // Email
        "Subject": "Order Confirmation #{{ orderId }}",
        "Body": "<h1>Order Confirmation</h1><p>Dear Customer,</p><p>Thank you for your order! Your order #{{ orderId }} has been successfully placed on {{ placedAt | date.to_string '%Y-%m-%d %H:%M' }}.</p><p><strong>Order Total: ${{ totalAmount }}</strong></p><p>We'll send you another email once your order ships.</p><p>Best regards,<br>Your Store Team</p>",
        "Language": "en",
        "Parameters": {
            "orderId": "Order ID",
            "totalAmount": "Order total amount",
            "placedAt": "Order placement date"
        },
        "IsActive": true,
        "CreatedAt": new Date(),
        "UpdatedAt": new Date(),
        "CreatedBy": "System",
        "UpdatedBy": "System"
    },
    {
        "_id": ObjectId(),
        "Name": "password_reset_requested_notification",
        "Type": 1, // Email
        "Subject": "Password Reset Request",
        "Body": "<h1>Password Reset</h1><p>We received a request to reset your password on {{ requestedAt | date.to_string '%Y-%m-%d %H:%M' }}.</p><p>Click the link below to reset your password:</p><p><a href='{{ reset_url }}?token={{ resetToken }}'>Reset Password</a></p><p>If you didn't request this, please ignore this email.</p><p>Best regards,<br>Support Team</p>",
        "Language": "en",
        "Parameters": {
            "resetToken": "Password reset token",
            "requestedAt": "Reset request date",
            "reset_url": "Password reset URL"
        },
        "IsActive": true,
        "CreatedAt": new Date(),
        "UpdatedAt": new Date(),
        "CreatedBy": "System",
        "UpdatedBy": "System"
    }
]);

print('NotificationService database initialized successfully!');
