import React from 'react'
import { Link } from 'react-router-dom'
import { MapPin, Building2, Users } from 'lucide-react'

/**
 * Component CompanyCard - Thẻ hiển thị thông tin công ty
 * @param {Object} props
 * @param {Object} props.company - Thông tin công ty
 */
const CompanyCard = ({ company }) => {
    return (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 hover:shadow-md transition-shadow">
            <div className="flex items-start gap-4">
                {/* Logo công ty */}
                <div className="w-16 h-16 rounded-lg bg-gray-50 flex items-center justify-center border border-gray-100 overflow-hidden flex-shrink-0">
                    {company.logoUrl ? (
                        <img
                            src={company.logoUrl}
                            alt={`Logo ${company.name}`}
                            className="w-full h-full object-contain"
                        />
                    ) : (
                        <span className="text-2xl font-bold text-primary-600">
                            {company.name ? company.name.charAt(0) : 'C'}
                        </span>
                    )}
                </div>

                <div className="flex-1">
                    <h3 className="text-lg font-semibold text-gray-900 mb-1">
                        {company.name}
                    </h3>

                    <div className="flex flex-wrap gap-y-2 gap-x-4 mb-4 text-sm text-gray-500">
                        {company.industry && (
                            <div className="flex items-center gap-1">
                                <Building2 className="w-4 h-4" />
                                <span>{company.industry}</span>
                            </div>
                        )}
                        {company.address && (
                            <div className="flex items-center gap-1">
                                <MapPin className="w-4 h-4" />
                                <span>{company.address}</span>
                            </div>
                        )}
                        {company.employeeCount && (
                            <div className="flex items-center gap-1">
                                <Users className="w-4 h-4" />
                                <span>{company.employeeCount} nhân viên</span>
                            </div>
                        )}
                    </div>

                    <div className="flex items-center justify-end">
                        <Link
                            to={`/companies/${company.id}`}
                            className="px-4 py-2 bg-primary-50 text-primary-600 text-sm font-semibold rounded-lg hover:bg-primary-100 transition-colors"
                        >
                            Xem chi tiết
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default CompanyCard
