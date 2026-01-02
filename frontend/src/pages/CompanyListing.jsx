/**
 * =============================================================================
 * COMPANY LISTING PAGE - Trang danh sách công ty
 * =============================================================================
 */

import { useState, useEffect } from 'react'
import { useSearchParams } from 'react-router-dom'
import { Search } from 'lucide-react'
import Header from '../components/Header'
import CompanyCard from '../components/CompanyCard'
import { companiesApi } from '../services/api'

const CompanyListing = () => {
    const [searchParams, setSearchParams] = useSearchParams()

    // State
    const [companies, setCompanies] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [pagination, setPagination] = useState({
        pageNumber: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0
    })

    // Local filter state
    const [searchTerm, setSearchTerm] = useState(searchParams.get('searchTerm') || '')

    // Effect fetch data
    useEffect(() => {
        const fetchCompanies = async () => {
            setLoading(true)
            try {
                const params = {
                    searchTerm: searchParams.get('searchTerm'),
                    pageNumber: parseInt(searchParams.get('page') || '1'),
                    pageSize: 10,
                }

                // Call API
                const response = await companiesApi.search(params)

                if (response.data.success) {
                    setCompanies(response.data.data.items)
                    setPagination({
                        pageNumber: response.data.data.pageNumber,
                        pageSize: 10,
                        totalCount: response.data.data.totalCount,
                        totalPages: response.data.data.totalPages
                    })
                }
            } catch (err) {
                console.error('Lỗi tải danh sách công ty:', err)
                setError('Không thể tải danh sách công ty.')
            } finally {
                setLoading(false)
            }
        }

        fetchCompanies()
    }, [searchParams])

    // Handle search
    const handleSearch = (e) => {
        e.preventDefault()
        const params = {}
        if (searchTerm) params.searchTerm = searchTerm
        setSearchParams(params)
    }

    return (
        <div className="min-h-screen bg-gray-50">
            <Header />

            {/* Search Bar */}
            <div className="bg-white border-b border-gray-200 sticky top-0 z-10">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
                    <form onSubmit={handleSearch} className="flex gap-4">
                        <div className="flex-1 relative">
                            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                            <input
                                type="text"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                placeholder="Tìm kiếm công ty..."
                                className="w-full pl-10 pr-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500"
                            />
                        </div>
                        <button
                            type="submit"
                            className="px-6 py-2.5 bg-primary-600 text-white font-medium rounded-lg hover:bg-primary-700 transition-colors"
                        >
                            Tìm kiếm
                        </button>
                    </form>
                </div>
            </div>

            {/* Main Content */}
            <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="mb-4 flex justify-between items-center">
                    <h1 className="text-xl font-bold text-gray-900">
                        {loading ? 'Đang tải...' : `${pagination.totalCount} công ty`}
                    </h1>
                </div>

                {loading ? (
                    // Skeleton
                    <div className="space-y-4">
                        {[1, 2, 3].map((n) => (
                            <div key={n} className="bg-white p-6 rounded-xl border border-gray-100 animate-pulse">
                                <div className="h-16 w-16 bg-gray-200 rounded-lg"></div>
                            </div>
                        ))}
                    </div>
                ) : error ? (
                    <div className="text-center py-12 text-red-500">{error}</div>
                ) : companies.length > 0 ? (
                    <div className="grid gap-4">
                        {companies.map(company => (
                            <CompanyCard key={company.id} company={company} />
                        ))}
                    </div>
                ) : (
                    <div className="text-center py-16 text-gray-500">
                        Không tìm thấy công ty nào.
                    </div>
                )}
            </main>
        </div>
    )
}

export default CompanyListing
