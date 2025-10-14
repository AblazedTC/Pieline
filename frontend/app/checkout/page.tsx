"use client"

import type React from "react"

import { useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import { Separator } from "@/components/ui/separator"
import { Badge } from "@/components/ui/badge"
import { Pizza, CreditCard, Wallet, MapPin, Tag } from "lucide-react"
import { Alert, AlertDescription } from "@/components/ui/alert"

// Mock cart data - in real app this would come from context/state
const mockCartItems = [
  {
    id: "1",
    name: "Margherita",
    quantity: 2,
    price: 12.99,
  },
  {
    id: "2",
    name: "Pepperoni",
    quantity: 1,
    price: 14.99,
  },
]

export default function CheckoutPage() {
  const router = useRouter()
  const [step, setStep] = useState<"address" | "payment" | "review">("address")
  const [couponCode, setCouponCode] = useState("")
  const [appliedCoupon, setAppliedCoupon] = useState<{ code: string; discount: number } | null>(null)
  const [isProcessing, setIsProcessing] = useState(false)

  // Address form state
  const [address, setAddress] = useState({
    street: "",
    city: "",
    state: "",
    zipCode: "",
  })

  // Payment form state
  const [paymentMethod, setPaymentMethod] = useState<"card" | "cash">("card")
  const [cardDetails, setCardDetails] = useState({
    number: "",
    name: "",
    expiry: "",
    cvv: "",
  })

  const subtotal = mockCartItems.reduce((sum, item) => sum + item.price * item.quantity, 0)
  const discount = appliedCoupon ? (subtotal * appliedCoupon.discount) / 100 : 0
  const tax = (subtotal - discount) * 0.08
  const deliveryFee = 3.99
  const total = subtotal - discount + tax + deliveryFee

  const handleApplyCoupon = () => {
    // Mock coupon validation
    if (couponCode.toUpperCase() === "SAVE10") {
      setAppliedCoupon({ code: couponCode, discount: 10 })
    } else if (couponCode.toUpperCase() === "SAVE20") {
      setAppliedCoupon({ code: couponCode, discount: 20 })
    }
  }

  const handleAddressSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setStep("payment")
  }

  const handlePaymentSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setStep("review")
  }

  const handlePlaceOrder = async () => {
    setIsProcessing(true)
    // Simulate order processing
    setTimeout(() => {
      router.push("/order-confirmation")
    }, 2000)
  }

  return (
    <div className="min-h-screen flex flex-col bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <Link href="/" className="flex items-center gap-2">
            <Pizza className="h-8 w-8 text-primary" />
            <span className="text-2xl font-bold text-foreground">PieLine</span>
          </Link>
          <div className="flex items-center gap-2">
            <Badge variant={step === "address" ? "default" : "secondary"}>1. Address</Badge>
            <Badge variant={step === "payment" ? "default" : "secondary"}>2. Payment</Badge>
            <Badge variant={step === "review" ? "default" : "secondary"}>3. Review</Badge>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 container mx-auto px-4 py-8">
        <div className="max-w-6xl mx-auto">
          <h1 className="text-4xl font-bold mb-8 text-foreground">Checkout</h1>

          <div className="grid lg:grid-cols-3 gap-8">
            {/* Checkout Forms */}
            <div className="lg:col-span-2 space-y-6">
              {/* Address Step */}
              {step === "address" && (
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <MapPin className="h-5 w-5" />
                      Delivery Address
                    </CardTitle>
                    <CardDescription>Where should we deliver your order?</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handleAddressSubmit} className="space-y-4">
                      <div className="space-y-2">
                        <Label htmlFor="street">Street Address</Label>
                        <Input
                          id="street"
                          placeholder="123 Main St"
                          value={address.street}
                          onChange={(e) => setAddress({ ...address, street: e.target.value })}
                          required
                        />
                      </div>
                      <div className="grid grid-cols-2 gap-4">
                        <div className="space-y-2">
                          <Label htmlFor="city">City</Label>
                          <Input
                            id="city"
                            placeholder="New York"
                            value={address.city}
                            onChange={(e) => setAddress({ ...address, city: e.target.value })}
                            required
                          />
                        </div>
                        <div className="space-y-2">
                          <Label htmlFor="state">State</Label>
                          <Input
                            id="state"
                            placeholder="NY"
                            value={address.state}
                            onChange={(e) => setAddress({ ...address, state: e.target.value })}
                            required
                          />
                        </div>
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="zipCode">ZIP Code</Label>
                        <Input
                          id="zipCode"
                          placeholder="10001"
                          value={address.zipCode}
                          onChange={(e) => setAddress({ ...address, zipCode: e.target.value })}
                          required
                        />
                      </div>
                      <Button type="submit" className="w-full">
                        Continue to Payment
                      </Button>
                    </form>
                  </CardContent>
                </Card>
              )}

              {/* Payment Step */}
              {step === "payment" && (
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <CreditCard className="h-5 w-5" />
                      Payment Method
                    </CardTitle>
                    <CardDescription>Choose how you'd like to pay</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handlePaymentSubmit} className="space-y-6">
                      <RadioGroup
                        value={paymentMethod}
                        onValueChange={(value: "card" | "cash") => setPaymentMethod(value)}
                      >
                        <div className="flex items-center space-x-3 p-4 border border-border rounded-lg">
                          <RadioGroupItem value="card" id="card" />
                          <Label htmlFor="card" className="flex items-center gap-2 cursor-pointer flex-1">
                            <CreditCard className="h-5 w-5" />
                            Credit/Debit Card
                          </Label>
                        </div>
                        <div className="flex items-center space-x-3 p-4 border border-border rounded-lg">
                          <RadioGroupItem value="cash" id="cash" />
                          <Label htmlFor="cash" className="flex items-center gap-2 cursor-pointer flex-1">
                            <Wallet className="h-5 w-5" />
                            Cash on Delivery
                          </Label>
                        </div>
                      </RadioGroup>

                      {paymentMethod === "card" && (
                        <div className="space-y-4 pt-4">
                          <div className="space-y-2">
                            <Label htmlFor="cardNumber">Card Number</Label>
                            <Input
                              id="cardNumber"
                              placeholder="1234 5678 9012 3456"
                              value={cardDetails.number}
                              onChange={(e) => setCardDetails({ ...cardDetails, number: e.target.value })}
                              required
                            />
                          </div>
                          <div className="space-y-2">
                            <Label htmlFor="cardName">Cardholder Name</Label>
                            <Input
                              id="cardName"
                              placeholder="John Doe"
                              value={cardDetails.name}
                              onChange={(e) => setCardDetails({ ...cardDetails, name: e.target.value })}
                              required
                            />
                          </div>
                          <div className="grid grid-cols-2 gap-4">
                            <div className="space-y-2">
                              <Label htmlFor="expiry">Expiry Date</Label>
                              <Input
                                id="expiry"
                                placeholder="MM/YY"
                                value={cardDetails.expiry}
                                onChange={(e) => setCardDetails({ ...cardDetails, expiry: e.target.value })}
                                required
                              />
                            </div>
                            <div className="space-y-2">
                              <Label htmlFor="cvv">CVV</Label>
                              <Input
                                id="cvv"
                                placeholder="123"
                                value={cardDetails.cvv}
                                onChange={(e) => setCardDetails({ ...cardDetails, cvv: e.target.value })}
                                required
                              />
                            </div>
                          </div>
                        </div>
                      )}

                      <div className="flex gap-3">
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => setStep("address")}
                          className="flex-1 bg-transparent"
                        >
                          Back
                        </Button>
                        <Button type="submit" className="flex-1">
                          Review Order
                        </Button>
                      </div>
                    </form>
                  </CardContent>
                </Card>
              )}

              {/* Review Step */}
              {step === "review" && (
                <div className="space-y-6">
                  <Card>
                    <CardHeader>
                      <CardTitle>Delivery Address</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <p className="text-foreground">
                        {address.street}
                        <br />
                        {address.city}, {address.state} {address.zipCode}
                      </p>
                      <Button variant="link" className="px-0 mt-2" onClick={() => setStep("address")}>
                        Edit Address
                      </Button>
                    </CardContent>
                  </Card>

                  <Card>
                    <CardHeader>
                      <CardTitle>Payment Method</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <p className="text-foreground">
                        {paymentMethod === "card"
                          ? `Card ending in ${cardDetails.number.slice(-4)}`
                          : "Cash on Delivery"}
                      </p>
                      <Button variant="link" className="px-0 mt-2" onClick={() => setStep("payment")}>
                        Edit Payment
                      </Button>
                    </CardContent>
                  </Card>

                  <Button className="w-full" size="lg" onClick={handlePlaceOrder} disabled={isProcessing}>
                    {isProcessing ? "Processing..." : `Place Order - $${total.toFixed(2)}`}
                  </Button>
                </div>
              )}
            </div>

            {/* Order Summary */}
            <div className="lg:col-span-1">
              <Card className="sticky top-24">
                <CardHeader>
                  <CardTitle>Order Summary</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  {/* Cart Items */}
                  <div className="space-y-3">
                    {mockCartItems.map((item) => (
                      <div key={item.id} className="flex justify-between text-sm">
                        <span className="text-foreground">
                          {item.name} x{item.quantity}
                        </span>
                        <span className="font-medium">${(item.price * item.quantity).toFixed(2)}</span>
                      </div>
                    ))}
                  </div>

                  <Separator />

                  {/* Coupon Code */}
                  <div className="space-y-2">
                    <Label htmlFor="coupon" className="flex items-center gap-2">
                      <Tag className="h-4 w-4" />
                      Coupon Code
                    </Label>
                    <div className="flex gap-2">
                      <Input
                        id="coupon"
                        placeholder="Enter code"
                        value={couponCode}
                        onChange={(e) => setCouponCode(e.target.value)}
                        disabled={!!appliedCoupon}
                      />
                      <Button
                        variant="outline"
                        onClick={handleApplyCoupon}
                        disabled={!!appliedCoupon || !couponCode}
                        className="bg-transparent"
                      >
                        Apply
                      </Button>
                    </div>
                    {appliedCoupon && (
                      <Alert className="bg-secondary/50 border-secondary">
                        <AlertDescription className="text-secondary-foreground text-sm">
                          Coupon "{appliedCoupon.code}" applied! {appliedCoupon.discount}% off
                        </AlertDescription>
                      </Alert>
                    )}
                  </div>

                  <Separator />

                  {/* Price Breakdown */}
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Subtotal</span>
                      <span className="font-medium">${subtotal.toFixed(2)}</span>
                    </div>
                    {appliedCoupon && (
                      <div className="flex justify-between text-sm">
                        <span className="text-muted-foreground">Discount</span>
                        <span className="font-medium text-secondary">-${discount.toFixed(2)}</span>
                      </div>
                    )}
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Tax (8%)</span>
                      <span className="font-medium">${tax.toFixed(2)}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-muted-foreground">Delivery Fee</span>
                      <span className="font-medium">${deliveryFee.toFixed(2)}</span>
                    </div>
                    <Separator />
                    <div className="flex justify-between text-lg font-bold">
                      <span>Total</span>
                      <span className="text-primary">${total.toFixed(2)}</span>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
        </div>
      </main>
    </div>
  )
}
