"use client"

import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { CheckCircle2, Pizza, MapPin, CreditCard, Clock } from "lucide-react"

// Mock order data
const orderDetails = {
  orderNumber: "ORD-" + Math.random().toString(36).substr(2, 9).toUpperCase(),
  estimatedTime: "30-40 minutes",
  items: [
    { name: "Margherita", quantity: 2, price: 12.99 },
    { name: "Pepperoni", quantity: 1, price: 14.99 },
  ],
  address: "123 Main St, New York, NY 10001",
  paymentMethod: "Card ending in 3456",
  subtotal: 40.97,
  discount: 4.1,
  tax: 2.95,
  deliveryFee: 3.99,
  total: 43.81,
}

export default function OrderConfirmationPage() {
  return (
    <div className="min-h-screen flex flex-col bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <Link href="/" className="flex items-center gap-2">
            <Pizza className="h-8 w-8 text-primary" />
            <span className="text-2xl font-bold text-foreground">PieLine</span>
          </Link>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 container mx-auto px-4 py-12">
        <div className="max-w-3xl mx-auto">
          {/* Success Message */}
          <div className="text-center mb-8">
            <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-secondary/20 mb-4">
              <CheckCircle2 className="h-10 w-10 text-secondary" />
            </div>
            <h1 className="text-4xl font-bold mb-2 text-foreground">Order Confirmed!</h1>
            <p className="text-xl text-muted-foreground">Thank you for your order</p>
          </div>

          {/* Order Details */}
          <Card className="mb-6">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle>Order #{orderDetails.orderNumber}</CardTitle>
                  <CardDescription className="flex items-center gap-2 mt-1">
                    <Clock className="h-4 w-4" />
                    Estimated delivery: {orderDetails.estimatedTime}
                  </CardDescription>
                </div>
              </div>
            </CardHeader>
            <CardContent className="space-y-6">
              {/* Order Items */}
              <div>
                <h3 className="font-semibold mb-3 text-foreground">Order Items</h3>
                <div className="space-y-2">
                  {orderDetails.items.map((item, index) => (
                    <div key={index} className="flex justify-between text-sm">
                      <span className="text-foreground">
                        {item.name} x{item.quantity}
                      </span>
                      <span className="font-medium">${(item.price * item.quantity).toFixed(2)}</span>
                    </div>
                  ))}
                </div>
              </div>

              <Separator />

              {/* Delivery Address */}
              <div>
                <h3 className="font-semibold mb-2 flex items-center gap-2 text-foreground">
                  <MapPin className="h-4 w-4" />
                  Delivery Address
                </h3>
                <p className="text-sm text-muted-foreground">{orderDetails.address}</p>
              </div>

              <Separator />

              {/* Payment Method */}
              <div>
                <h3 className="font-semibold mb-2 flex items-center gap-2 text-foreground">
                  <CreditCard className="h-4 w-4" />
                  Payment Method
                </h3>
                <p className="text-sm text-muted-foreground">{orderDetails.paymentMethod}</p>
              </div>

              <Separator />

              {/* Price Summary */}
              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Subtotal</span>
                  <span className="font-medium">${orderDetails.subtotal.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Discount</span>
                  <span className="font-medium text-secondary">-${orderDetails.discount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Tax</span>
                  <span className="font-medium">${orderDetails.tax.toFixed(2)}</span>
                </div>
                <div className="flex justify-between text-sm">
                  <span className="text-muted-foreground">Delivery Fee</span>
                  <span className="font-medium">${orderDetails.deliveryFee.toFixed(2)}</span>
                </div>
                <Separator />
                <div className="flex justify-between text-lg font-bold">
                  <span>Total Paid</span>
                  <span className="text-primary">${orderDetails.total.toFixed(2)}</span>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Action Buttons */}
          <div className="flex flex-col sm:flex-row gap-3">
            <Button asChild className="flex-1">
              <Link href="/menu">Order Again</Link>
            </Button>
            <Button variant="outline" asChild className="flex-1 bg-transparent">
              <Link href="/account">View Order History</Link>
            </Button>
          </div>
        </div>
      </main>
    </div>
  )
}
