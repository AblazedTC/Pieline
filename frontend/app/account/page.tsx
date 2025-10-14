"use client"

import type React from "react"

import { useState } from "react"
import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Badge } from "@/components/ui/badge"
import { Separator } from "@/components/ui/separator"
import { Pizza, User, MapPin, CreditCard, History, Settings, LogOut } from "lucide-react"
import { Alert, AlertDescription } from "@/components/ui/alert"

// Mock user data
const mockUser = {
  name: "John Doe",
  email: "john.doe@example.com",
  phone: "(555) 123-4567",
}

// Mock addresses
const mockAddresses = [
  {
    id: "1",
    street: "123 Main St",
    city: "New York",
    state: "NY",
    zipCode: "10001",
    isDefault: true,
  },
  {
    id: "2",
    street: "456 Oak Ave",
    city: "Brooklyn",
    state: "NY",
    zipCode: "11201",
    isDefault: false,
  },
]

// Mock payment methods
const mockPaymentMethods = [
  {
    id: "1",
    type: "card" as const,
    last4: "3456",
    isDefault: true,
  },
  {
    id: "2",
    type: "card" as const,
    last4: "7890",
    isDefault: false,
  },
]

// Mock order history
const mockOrders = [
  {
    id: "ORD-ABC123",
    date: "2025-01-15",
    items: ["Margherita x2", "Pepperoni x1"],
    total: 43.81,
    status: "delivered" as const,
  },
  {
    id: "ORD-DEF456",
    date: "2025-01-10",
    items: ["BBQ Chicken x1", "Veggie Supreme x1"],
    total: 35.99,
    status: "delivered" as const,
  },
  {
    id: "ORD-GHI789",
    date: "2025-01-05",
    items: ["Meat Lovers x2"],
    total: 39.98,
    status: "delivered" as const,
  },
]

export default function AccountPage() {
  const [user, setUser] = useState(mockUser)
  const [isEditingProfile, setIsEditingProfile] = useState(false)
  const [profileForm, setProfileForm] = useState(mockUser)
  const [passwordForm, setPasswordForm] = useState({
    current: "",
    new: "",
    confirm: "",
  })
  const [successMessage, setSuccessMessage] = useState("")

  const handleProfileUpdate = (e: React.FormEvent) => {
    e.preventDefault()
    setUser(profileForm)
    setIsEditingProfile(false)
    setSuccessMessage("Profile updated successfully!")
    setTimeout(() => setSuccessMessage(""), 3000)
  }

  const handlePasswordChange = (e: React.FormEvent) => {
    e.preventDefault()
    if (passwordForm.new !== passwordForm.confirm) {
      return
    }
    setPasswordForm({ current: "", new: "", confirm: "" })
    setSuccessMessage("Password changed successfully!")
    setTimeout(() => setSuccessMessage(""), 3000)
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
          <Button variant="ghost" asChild>
            <Link href="/menu">Back to Menu</Link>
          </Button>
        </div>
      </header>

      {/* Main Content */}
      <main className="flex-1 container mx-auto px-4 py-8">
        <div className="max-w-6xl mx-auto">
          <div className="flex items-center justify-between mb-8">
            <div>
              <h1 className="text-4xl font-bold text-foreground">My Account</h1>
              <p className="text-muted-foreground mt-1">Manage your profile and orders</p>
            </div>
            <Button variant="outline" className="bg-transparent">
              <LogOut className="h-4 w-4 mr-2" />
              Sign Out
            </Button>
          </div>

          {successMessage && (
            <Alert className="mb-6 bg-secondary/50 border-secondary">
              <AlertDescription className="text-secondary-foreground">{successMessage}</AlertDescription>
            </Alert>
          )}

          <Tabs defaultValue="profile" className="space-y-6">
            <TabsList className="grid w-full grid-cols-4 lg:w-auto lg:inline-grid">
              <TabsTrigger value="profile" className="flex items-center gap-2">
                <User className="h-4 w-4" />
                <span className="hidden sm:inline">Profile</span>
              </TabsTrigger>
              <TabsTrigger value="addresses" className="flex items-center gap-2">
                <MapPin className="h-4 w-4" />
                <span className="hidden sm:inline">Addresses</span>
              </TabsTrigger>
              <TabsTrigger value="payment" className="flex items-center gap-2">
                <CreditCard className="h-4 w-4" />
                <span className="hidden sm:inline">Payment</span>
              </TabsTrigger>
              <TabsTrigger value="orders" className="flex items-center gap-2">
                <History className="h-4 w-4" />
                <span className="hidden sm:inline">Orders</span>
              </TabsTrigger>
            </TabsList>

            {/* Profile Tab */}
            <TabsContent value="profile" className="space-y-6">
              <Card>
                <CardHeader>
                  <CardTitle>Personal Information</CardTitle>
                  <CardDescription>Update your account details</CardDescription>
                </CardHeader>
                <CardContent>
                  {isEditingProfile ? (
                    <form onSubmit={handleProfileUpdate} className="space-y-4">
                      <div className="space-y-2">
                        <Label htmlFor="name">Full Name</Label>
                        <Input
                          id="name"
                          value={profileForm.name}
                          onChange={(e) => setProfileForm({ ...profileForm, name: e.target.value })}
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="email">Email</Label>
                        <Input
                          id="email"
                          type="email"
                          value={profileForm.email}
                          onChange={(e) => setProfileForm({ ...profileForm, email: e.target.value })}
                          required
                        />
                      </div>
                      <div className="space-y-2">
                        <Label htmlFor="phone">Phone Number</Label>
                        <Input
                          id="phone"
                          type="tel"
                          value={profileForm.phone}
                          onChange={(e) => setProfileForm({ ...profileForm, phone: e.target.value })}
                          required
                        />
                      </div>
                      <div className="flex gap-3">
                        <Button type="submit">Save Changes</Button>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => {
                            setIsEditingProfile(false)
                            setProfileForm(user)
                          }}
                          className="bg-transparent"
                        >
                          Cancel
                        </Button>
                      </div>
                    </form>
                  ) : (
                    <div className="space-y-4">
                      <div>
                        <Label className="text-muted-foreground">Full Name</Label>
                        <p className="text-lg font-medium text-foreground">{user.name}</p>
                      </div>
                      <div>
                        <Label className="text-muted-foreground">Email</Label>
                        <p className="text-lg font-medium text-foreground">{user.email}</p>
                      </div>
                      <div>
                        <Label className="text-muted-foreground">Phone Number</Label>
                        <p className="text-lg font-medium text-foreground">{user.phone}</p>
                      </div>
                      <Button onClick={() => setIsEditingProfile(true)}>
                        <Settings className="h-4 w-4 mr-2" />
                        Edit Profile
                      </Button>
                    </div>
                  )}
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Change Password</CardTitle>
                  <CardDescription>Update your password to keep your account secure</CardDescription>
                </CardHeader>
                <CardContent>
                  <form onSubmit={handlePasswordChange} className="space-y-4">
                    <div className="space-y-2">
                      <Label htmlFor="currentPassword">Current Password</Label>
                      <Input
                        id="currentPassword"
                        type="password"
                        value={passwordForm.current}
                        onChange={(e) => setPasswordForm({ ...passwordForm, current: e.target.value })}
                        required
                      />
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="newPassword">New Password</Label>
                      <Input
                        id="newPassword"
                        type="password"
                        value={passwordForm.new}
                        onChange={(e) => setPasswordForm({ ...passwordForm, new: e.target.value })}
                        required
                      />
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="confirmPassword">Confirm New Password</Label>
                      <Input
                        id="confirmPassword"
                        type="password"
                        value={passwordForm.confirm}
                        onChange={(e) => setPasswordForm({ ...passwordForm, confirm: e.target.value })}
                        required
                      />
                    </div>
                    <Button type="submit">Change Password</Button>
                  </form>
                </CardContent>
              </Card>
            </TabsContent>

            {/* Addresses Tab */}
            <TabsContent value="addresses" className="space-y-6">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-2xl font-bold text-foreground">Saved Addresses</h2>
                  <p className="text-muted-foreground">Manage your delivery addresses</p>
                </div>
                <Button>Add New Address</Button>
              </div>

              <div className="grid md:grid-cols-2 gap-4">
                {mockAddresses.map((address) => (
                  <Card key={address.id}>
                    <CardHeader>
                      <div className="flex items-start justify-between">
                        <CardTitle className="text-lg">
                          {address.isDefault && (
                            <Badge variant="secondary" className="mr-2">
                              Default
                            </Badge>
                          )}
                          Address
                        </CardTitle>
                      </div>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <p className="text-foreground">
                        {address.street}
                        <br />
                        {address.city}, {address.state} {address.zipCode}
                      </p>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm" className="bg-transparent">
                          Edit
                        </Button>
                        {!address.isDefault && (
                          <Button variant="outline" size="sm" className="bg-transparent">
                            Set as Default
                          </Button>
                        )}
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            </TabsContent>

            {/* Payment Tab */}
            <TabsContent value="payment" className="space-y-6">
              <div className="flex items-center justify-between">
                <div>
                  <h2 className="text-2xl font-bold text-foreground">Payment Methods</h2>
                  <p className="text-muted-foreground">Manage your saved payment methods</p>
                </div>
                <Button>Add New Card</Button>
              </div>

              <div className="grid md:grid-cols-2 gap-4">
                {mockPaymentMethods.map((method) => (
                  <Card key={method.id}>
                    <CardHeader>
                      <div className="flex items-start justify-between">
                        <CardTitle className="text-lg flex items-center gap-2">
                          <CreditCard className="h-5 w-5" />
                          {method.isDefault && (
                            <Badge variant="secondary" className="ml-2">
                              Default
                            </Badge>
                          )}
                        </CardTitle>
                      </div>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <p className="text-foreground">Card ending in {method.last4}</p>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm" className="bg-transparent">
                          Edit
                        </Button>
                        {!method.isDefault && (
                          <Button variant="outline" size="sm" className="bg-transparent">
                            Set as Default
                          </Button>
                        )}
                        <Button variant="outline" size="sm" className="bg-transparent">
                          Remove
                        </Button>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            </TabsContent>

            {/* Orders Tab */}
            <TabsContent value="orders" className="space-y-6">
              <div>
                <h2 className="text-2xl font-bold text-foreground">Order History</h2>
                <p className="text-muted-foreground">View your past orders</p>
              </div>

              <div className="space-y-4">
                {mockOrders.map((order) => (
                  <Card key={order.id}>
                    <CardHeader>
                      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-2">
                        <div>
                          <CardTitle className="text-lg">Order #{order.id}</CardTitle>
                          <CardDescription>{new Date(order.date).toLocaleDateString()}</CardDescription>
                        </div>
                        <Badge
                          variant={order.status === "delivered" ? "secondary" : "default"}
                          className="w-fit capitalize"
                        >
                          {order.status}
                        </Badge>
                      </div>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <div>
                        <Label className="text-muted-foreground text-sm">Items</Label>
                        <p className="text-foreground">{order.items.join(", ")}</p>
                      </div>
                      <Separator />
                      <div className="flex items-center justify-between">
                        <span className="font-semibold text-foreground">Total</span>
                        <span className="text-lg font-bold text-primary">${order.total.toFixed(2)}</span>
                      </div>
                      <div className="flex gap-2">
                        <Button variant="outline" size="sm" className="bg-transparent">
                          View Receipt
                        </Button>
                        <Button size="sm">Reorder</Button>
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            </TabsContent>
          </Tabs>
        </div>
      </main>
    </div>
  )
}
