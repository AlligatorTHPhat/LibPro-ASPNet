import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginForm from './components/auth/LoginForm';
import RegisterReaderForm from './components/auth/RegisterReaderForm';
import AdminLayout from './components/Layout/AdminLayout';
import LibrarianLayout from './components/Layout/LibrarianLayout';
import ReaderLayout from './components/Layout/ReaderLayout';

import CategoryManagement from './components/categories/CategoryManagement';
import BookManagement from './components/books/BookManagement';
import ReaderManagement from './components/readers/ReaderManagement';
import LibrarianLoanManagement from './components/loans/LibrarianLoanManagement'
import PublisherManagement from './components/publishers/PublisherManagement';
import SupplierManagement from './components/suppliers/SupplierManagement';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './ProtectedRoute';
import UserProfile from './pages/UserProfile';

import BookGallery from './components/books/BookGallery';
import ReaderLoanHistory from './components/loans/ReaderLoanHistory';

import ReaderReservationHistory from './components/reservations/ReaderReservationHistory';
import ReservationManagement from './components/reservations/ReservationManagement';

import AdminDashboard from './components/audit/AdminDashboard';
import StaffManagement from './components/staffs/StaffManagment'
import SystemAuditLog from './components/audit/SystemAuditLog'

function App() {
    return (
        <AuthProvider>
            <div id="app">
                <div id="center">
                    <BrowserRouter>
                        <Routes>
                            {/* --- PUBLIC ROUTES --- */}
                            <Route path="/login" element={<LoginForm />} />
                            <Route path="/register" element={<RegisterReaderForm />} />
                            <Route path="/unauthorized" element={<div>Bạn không có quyền truy cập trang này!</div>} />

                            {/* --- VÙNG CHO DIRECTOR (ADMIN) --- */}
                            <Route element={<ProtectedRoute allowedRoles={['Director']} />}>
                                <Route path="/admin" element={<AdminLayout />}>
                                    <Route index element={<Navigate to="dashboard" />} />
                                    <Route path="dashboard" element={<AdminDashboard />} />
                                    <Route path="librarians" element={<StaffManagement />} />
                                    <Route path="system-logs" element={<SystemAuditLog />} />
                                    <Route path="profile" element={<UserProfile />} />
                                </Route>
                            </Route>

                            {/* --- 2. VÙNG CHO LIBRARIAN (THỦ THƯ) --- */}
                            <Route element={<ProtectedRoute allowedRoles={['Librarian']} />}>
                                <Route path="/librarian" element={<LibrarianLayout />}>
                                    <Route index element={<Navigate to="categories" />} /> {/* Tự động vào categories */}
                                    <Route path="categories" element={<CategoryManagement />} />
                                    <Route path="books" element={<BookManagement />} />
                                    <Route path="readers" element={<ReaderManagement />} />
                                    <Route path="reservations" element={<ReservationManagement />} />
                                    <Route path="loans" element={<LibrarianLoanManagement />} />
                                    <Route path="suppliers" element={<SupplierManagement />} />
                                    <Route path="publishers" element={<PublisherManagement />} />
                                    <Route path="profile" element={<UserProfile />} />
                                </Route>
                            </Route>

                            {/* --- 3. VÙNG CHO READER (ĐỘC GIẢ) --- */}
                            <Route element={<ProtectedRoute allowedRoles={['Reader']} />}>
                                <Route path="/reader" element={<ReaderLayout />}>
                                    <Route index element={<Navigate to="books" />}/> {/* Tự động vào danh sách sách */}
                                    <Route path="books" element={<BookGallery />} />
                                    <Route path="reservations" element={<ReaderReservationHistory />} />
                                    <Route path="my-loans" element={<ReaderLoanHistory />} />
                                    <Route path="profile" element={<UserProfile />} />
                                </Route>
                            </Route>

                            {/* --- ĐIỀU HƯỚNG MẶC ĐỊNH --- */}
                            <Route path="/" element={<Navigate to="/login" />} />
                            {/* Route 404 nếu người dùng nhập bừa link */}
                            <Route path="*" element={<Navigate to="/login" />} />
                        </Routes>
                    </BrowserRouter>
                </div>
            </div>
        </AuthProvider>
    );
}

export default App;