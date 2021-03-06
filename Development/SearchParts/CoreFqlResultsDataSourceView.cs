﻿using System;
using System.Linq;
using Microsoft.Office.Server.Search.Query;
using Microsoft.Office.Server.Search.WebControls;

namespace mAdcOW.SharePoint.Search
{
    /// <summary>
    /// Read in all fql created scopes
    /// Used for building fql with the correct data types
    ///
    /// Author: Mikael Svenson - mAdcOW deZign    
    /// E-mail: miksvenson@gmail.com
    /// Twitter: @mikaelsvenson
    /// 
    /// This source code is released under the MIT license
    /// 
    /// The code is based on code from http://neganov.blogspot.com/2011/01/extending-coreresultswebpart-to-handle.html
    /// </summary>
    internal class CoreFqlResultsDataSourceView : CoreResultsDatasourceView
    {
        private static readonly DynamicReflectionHelperforObject<Location>.GetPropertyFieldDelegate<ILocationRuntime>
            _internalGetHelper =
                DynamicReflectionHelperforObject<Location>.GetProperty<ILocationRuntime>("LocationRuntime");

        private CoreFqlResultsDataSource _fqlDataSourceOwner;

        public CoreFqlResultsDataSourceView(SearchResultsBaseDatasource dataSourceOwner, string viewName)
            : base(dataSourceOwner, viewName)
        {
            _fqlDataSourceOwner = base.DataSourceOwner as CoreFqlResultsDataSource;

            if (_fqlDataSourceOwner == null)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override void SetPropertiesOnQdra()
        {
            base.SetPropertiesOnQdra();
            // At this point the query has not yet been dispatched to a search 
            // location and we can set properties on that location, which will 
            // let it understand the FQL syntax.
            UpdateFastSearchLocation();
        }

        private void UpdateFastSearchLocation()
        {
            if (base.LocationList == null || 0 == base.LocationList.Count)
            {
                return;
            }

            foreach (var runtime in
                base.LocationList.Select(location => _internalGetHelper.Invoke(location)).OfType<FASTSearchRuntime>())
            {
                // This is a FAST Search runtime. We can now enable FQL.
                runtime.EnableFQL = _fqlDataSourceOwner.EnableFql;
                if (!string.IsNullOrEmpty(_fqlDataSourceOwner.DuplicateTrimProperty))
                    runtime.TrimDuplicatesOnProperty = _fqlDataSourceOwner.DuplicateTrimProperty;
                break;
            }
        }
    }
}